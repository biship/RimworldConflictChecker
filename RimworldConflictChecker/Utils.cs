using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace RimworldConflictChecker
{
    //You have to make it static or you must create an instance of the class and have a reference to that in order to call non-static methods.
    public static class Utils
    {
        public static bool FileOrDirectoryExists(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }
            try
            {
                return Directory.Exists(name) || File.Exists(name);
            }
            catch (Exception)
            {
                //throw;
                return false; //won't execute after a throw
            }
        }

        //conflicts with IEnumerable
        //// <summary>Indicates whether the specified array is null or has a length of zero.</summary>
        //// <typeparam name="collection"></typeparam>
        //// <param name="array">The array to test.</param>
        //// <returns>true if the array parameter is null or has a length of zero; otherwise, false.</returns>
        //public static bool IsNullOrEmpty(this Array array)
        //{
        //return (array == null || array.Length == 0); //true if null or empty
        //}

        public static bool IsNullOrEmpty<T>(this List<T> list)
        {
            return (list == null || list.Count == 0); //true if null or count = 0
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> ienumerable)
        {
            return (ienumerable == null || !ienumerable.Any()); //true if null or not any
        }

        public static bool ContainsAll(this string source, params string[] values)
        {
            //return values.All(x => source.Contains(x));
            return values.All(source.Contains);
        }

        public static bool IsFullPath(string path)
        {
            return !String.IsNullOrWhiteSpace(path)
                && path.IndexOfAny(System.IO.Path.GetInvalidPathChars().ToArray()) == -1
                && Path.IsPathRooted(path)
                && !Path.GetPathRoot(path).Equals(Path.DirectorySeparatorChar.ToString(), StringComparison.OrdinalIgnoreCase); //was just ordinal
        }

        public static void Each<T>(this IEnumerable<T> ie, Action<T, int> action)
        {
            var i = 0;
            foreach (var e in ie) action?.Invoke(e, i++);
        }

        public static TResult NullSafe<TObj, TResult>(this TObj obj, Func<TObj, TResult> func, TResult ifNullReturn = default(TResult))
        {
            return obj != null ? func(obj) : ifNullReturn;
        }

        public static T Capped<T>(this T @this, T? min = null, T? max = null) where T : struct, IComparable<T>
        {
            return min.HasValue && @this.CompareTo(min.Value) < 0 ? min.Value
                : max.HasValue && @this.CompareTo(max.Value) > 0 ? max.Value
                : @this;
        }

        public static Exception LogException(string what, Exception ex)
        {
            //File.AppendAllText("CaughtExceptions" + DateTime.Now.ToString("yyyy-MM-dd") + ".log", DateTime.Now.ToString("HH:mm:ss") + ": " + ex.Message + "\n" + ex.ToString() + "\n");
            Logger.Instance.LogError(what, ex);
            return ex;
        }

        public static Exception DisplayException(Exception ex, string msg = null, MessageBoxImage img = MessageBoxImage.Error)
        {
            MessageBox.Show(msg ?? ex.Message, "", MessageBoxButton.OK, img);
            return ex;
        }
    }


    public static class Throw<TException> where TException : Exception
    {
        public static void If(bool condition, string message)
        {
            if (condition)
            {
                throw (TException)Activator.CreateInstance(typeof(TException), message);
            }
        }
    }

    public static class ExampleCode
    {
        [Obsolete]
        private static void MethodName()
        {
            //NullSafe
            //foo.NullSafe(f => f.GetBar())
            //   .NullSafe(b => b.Baz)
            //   .NullSafe(b => b.SomeMethod(), ifNullReturn: "whatever you would return in the null case");

            //Capped:
            //var bound = value.Capped(min: 1, max: 10);
        }
    }
}
