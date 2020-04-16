using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace UsersMatcher.Models
{
    public class Album : IEquatable<Album>
    {
        public string Name { get; set; }

        public Artist Artist{ get; set; }
   
        public string Url { get; set; }

        public string Mbid { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Album);
        }

        public bool Equals(Album other)
        {
            return other != null &&
                   Name == other.Name &&
                   EqualityComparer<Artist>.Default.Equals(Artist, other.Artist) &&
                   Url == other.Url &&
                   Mbid == other.Mbid;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Artist, Url, Mbid);
        }

        public static bool operator ==(Album album1, Album album2)
        {
            return EqualityComparer<Album>.Default.Equals(album1, album2);
        }

        public static bool operator !=(Album album1, Album album2)
        {
            return !(album1 == album2);
        }
    }
}
