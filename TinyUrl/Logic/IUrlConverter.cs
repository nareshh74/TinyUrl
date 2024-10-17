namespace TinyUrl.Logic;

public interface IUrlConverter
{
    string Encode(Uri url);
}