using System.Windows.Input;

namespace ListFilesByDate.Model
{
    /// <summary>
    /// </summary>
    public class CalendarCommands
    {
        /// <summary>
        /// </summary>
        public static readonly RoutedCommand SelectToday = new("Today", typeof(CalendarCommands));
    }
}