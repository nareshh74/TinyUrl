import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
    stages: [
        { duration: '30s', target: 50 },  // Ramp-up to 50 users over 30 seconds
        { duration: '1m', target: 50 },   // Stay at 50 users for 1 minute
        { duration: '30s', target: 0 },   // Ramp-down to 0 users
    ],
};

export default function () {
    // Step 1: Call the shorten URL API
    const longUrlPayload = JSON.stringify({
        longUrl: "https://example.com"  // You can replace this with any URL
    });

    const shortenResponse = http.post('http://localhost:5091/api/v1/shorten', longUrlPayload, {
        headers: { 'Content-Type': 'application/json' },
    });

    // Check if the shorten URL API returns a 200 status code and has a shortUrl in the response
    check(shortenResponse, {
        'status was 200': (r) => r.status === 200,
        'shortUrl exists': (r) => JSON.parse(r.body).shortUrl !== undefined,
    });

    // Extract the shortUrl from the response
    const shortUrl = JSON.parse(shortenResponse.body).shortUrl;

    // Step 2: Call the short URL API using the shortUrl from the previous response
    const redirectResponse = http.get(`http://localhost:5091/api/v1/${shortUrl}`);

    // Check if the short URL redirects properly (status code 200 or 302 for redirect)
    check(redirectResponse, {
        'status was 200 or 302': (r) => r.status === 200 || r.status === 302,
    });

    // Add a small sleep to simulate user think time
    sleep(1);
}
