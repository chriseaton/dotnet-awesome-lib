/********************************************
 * MIT License
 * (c) Christopher Eaton, 2012
 * https://gitlab.com/chriseaton/awesome-lib
 ********************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Web;
using System.Web.Hosting;
using System.Web.Caching;
using System.Xml.Linq;
using System.IO;

namespace Awesome.Library.Mvc {

	public static class CacheManager {

		#region " Properties "

		private static Cache Cache {
			get {
				if ( HttpContext.Current != null ) {
					return HttpContext.Current.Cache;
				}
				return null;
			}
		}

		#endregion

		#region " Methods "

		public static void Remove( string key ) {
			if ( Cache != null ) {
				Cache.Remove( key );
			}
		}

		public static void Set<T>( string cacheKey, T value ) {
			Set<T>( cacheKey, value, null, Cache.NoAbsoluteExpiration, TimeSpan.FromHours( 8 ), CacheItemPriority.Normal, false );
		}

		public static void Set<T>( string cacheKey, T value, DateTime? absoluteExpiration, TimeSpan? slidingExpiration ) {
			Set<T>( cacheKey, value, null, absoluteExpiration, slidingExpiration, CacheItemPriority.Normal, false );
		}

		public static void Set<T>( string cacheKey, T value, DateTime? absoluteExpiration, TimeSpan? slidingExpiration, bool overwrite ) {
			Set<T>( cacheKey, value, null, absoluteExpiration, slidingExpiration, CacheItemPriority.Normal, overwrite );
		}

		public static void Set<T>( string cacheKey, T value, string path, DateTime? absoluteExpiration, TimeSpan? slidingExpiration ) {
			CacheDependency dep = null;
			if ( String.IsNullOrWhiteSpace( path ) == false ) {
				dep = new CacheDependency( path );
			}
			Set<T>( cacheKey, value, dep, absoluteExpiration, slidingExpiration, CacheItemPriority.Normal, false );
		}

		public static void Set<T>( string cacheKey, T value, CacheDependency dependancies, DateTime? absoluteExpiration, TimeSpan? slidingExpiration ) {
			Set<T>( cacheKey, value, dependancies, absoluteExpiration, slidingExpiration, CacheItemPriority.Normal, false );
		}

		public static void Set<T>( string cacheKey, T value, CacheDependency dependancies, DateTime? absoluteExpiration, TimeSpan? slidingExpiration, CacheItemPriority priority ) {
			Set<T>( cacheKey, value, dependancies, absoluteExpiration, slidingExpiration, priority, false );
		}

		public static void Set<T>( string cacheKey, T value, CacheDependency dependancies, DateTime? absoluteExpiration, TimeSpan? slidingExpiration, CacheItemPriority priority, bool overwrite ) {
			if ( CacheManager.Cache != null ) {
				if ( absoluteExpiration.HasValue == false ) {
					absoluteExpiration = Cache.NoAbsoluteExpiration;
				}
				if ( slidingExpiration.HasValue == false ) {
					slidingExpiration = Cache.NoSlidingExpiration;
				}
				if ( overwrite ) {
					if ( CacheManager.Cache[cacheKey] != null ) {
						CacheManager.Cache.Remove( cacheKey );
					}
					CacheManager.Cache.Add( cacheKey, value, dependancies, absoluteExpiration.Value, slidingExpiration.Value, priority, null );
				} else {
					if ( CacheManager.Cache[cacheKey] != null ) {
						CacheManager.Cache[cacheKey] = value;
					} else {
						CacheManager.Cache.Add( cacheKey, value, dependancies, absoluteExpiration.Value, slidingExpiration.Value, priority, null );
					}
				}
			}
		}

		public static T Get<T>( string cacheKey ) {
			if ( CacheManager.Cache != null && CacheManager.Cache[cacheKey] != null ) {
				return (T)CacheManager.Cache[cacheKey];
			}
			return default( T );
		}

		public static T LazyLoad<T>( Func<T> f, string cacheKey ) {
			return LazyLoad<T>( f, cacheKey, null, Cache.NoAbsoluteExpiration, TimeSpan.FromHours( 8 ), CacheItemPriority.Normal );
		}

		public static T LazyLoad<T>( Func<T> f, string cacheKey, DateTime absoluteExpiration, TimeSpan slidingExpiration ) {
			return LazyLoad<T>( f, cacheKey, null, absoluteExpiration, slidingExpiration, CacheItemPriority.Normal );
		}

		public static T LazyLoad<T>( Func<T> f, string cacheKey, CacheDependency dependancies, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemPriority priority ) {
			if ( Cache != null && Cache[cacheKey] != null ) {
				return (T)Cache[cacheKey];
			} else {
				T result = f();
				if ( Cache != null ) {
					Cache.Add( cacheKey, result, dependancies, absoluteExpiration, slidingExpiration, priority, null );
				}
				return result;
			}
		}

		#endregion

	}

}