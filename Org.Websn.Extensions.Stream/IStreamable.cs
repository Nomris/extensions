using System.IO;

namespace Org.Websn.Extensions
{
    public interface IStreamable
    {
        /// <summary>
        /// Writes the buffer to the <paramref name="stream"/>
        /// </summary>
        void ToStream(Stream stream);
    }
}
