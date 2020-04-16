using System;
using System.Collections.Generic;

namespace UsersMatcher.Models
{
    public class Artist : IEquatable<Artist>
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public string Mbid { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Artist);
        }

        public bool Equals(Artist other)
        {
            return other != null &&
                   Name == other.Name &&
                   Url == other.Url &&
                   Mbid == other.Mbid;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Url, Mbid);
        }

        public static bool operator ==(Artist artist1, Artist artist2)
        {
            return EqualityComparer<Artist>.Default.Equals(artist1, artist2);
        }

        public static bool operator !=(Artist artist1, Artist artist2)
        {
            return !(artist1 == artist2);
        }
    }
}