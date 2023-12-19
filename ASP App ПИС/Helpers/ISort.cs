namespace ASP_App_ПИС.Helpers
{
    public interface ISort
    {
        List<T> SortAsc<T>(List<T> list, string prop);
        List<T> SortDesc<T>(List<T> list, string prop);
    }

    public class SortByProp : ISort
    {
        public List<T> SortAsc<T>(List<T> list, string prop)
        {
            var newList = list.OrderBy(obj => {
                if (obj == null) return null;
                var s = obj.GetType().GetProperty(prop);
                return s.GetValue(obj);
            }).ToList();
            return newList;
        }

        public List<T> SortDesc<T>(List<T> list, string prop)
        {
            var newList =  list.OrderByDescending(obj => {
                if (obj == null) return null;
                var s = obj.GetType().GetProperty(prop);
                return s.GetValue(obj);
            }).ToList();
            return newList;
        }
    }

    public static class ListExtensions
    {
        public static List<T> SortAsc<T>(this List<T> list, ISort sort, string prop)
        {
            return sort.SortAsc(list, prop);
        }

        public static List<T> SortDesc<T>(this List<T> list, ISort sort, string prop)
        {
            return sort.SortDesc(list, prop);
        }
    }
}
