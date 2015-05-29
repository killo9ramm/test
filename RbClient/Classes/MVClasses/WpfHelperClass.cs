using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace RBClient.Classes
{
    class WpfHelperClass
    {
        /// <summary>
        /// Выбрать нужный элемент в листбоксе 
        /// </summary>
        /// <typeparam name="T">Элемент в котором происходит событие</typeparam>
        /// <typeparam name="U">Родительский элемент, который мы ищем</typeparam>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="list"></param>
        public static void SelectTaskItemClick<T, U>(object sender, RoutedEventArgs e, Selector list)
            where T : UIElement, new()
            where U : UIElement, new()
        {
            var b = sender as T;
            DependencyObject item = b;
            while (item is U == false)
            {
                item = VisualTreeHelper.GetParent(item);
            }
            ContentControl lbi = (ContentControl)item;
            list.SelectedItem = lbi.DataContext;
        }

        /// <summary>
        /// Выбрать нужный элемент в листбоксе 
        /// </summary>
        /// <typeparam name="T">Элемент в котором происходит событие</typeparam>
        /// <typeparam name="U">Родительский элемент, который мы ищем</typeparam>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="list"></param>
        public static U SelectTaskItemClick<T, U>(object sender, RoutedEventArgs e)
            where T : UIElement, new()
            where U : UIElement, new()
        {
            var b = sender as T;
            DependencyObject item = b;
            while (item is U == false)
            {
                if (item == null) return null;
                item = VisualTreeHelper.GetParent(item);
            }
            return item as U;
        }


        /// <summary>
        /// Родителя для элемента
        /// </summary>
        /// <param name="child">элемент</param>
        /// <param name="new_parent">новый родитель</param>
        public static void ChangeParentFor(object child, ContentControl new_parent)
        {
            if (((FrameworkElement)child).Parent is ContentControl)
            {
                ((ContentControl)((FrameworkElement)child).Parent).Content = null;
                new_parent.Content = child;
            }
        }


        //public static List<T> FindChilds<T>(DependencyObject parent) where T : DependencyObject
        //{
        //    // Confirm parent and childName are valid. 
        //    if (parent == null) return null;

        //    List<T> foundChild = null;

        //    int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
        //    for (int i = 0; i < childrenCount; i++)
        //    {
        //        var child = VisualTreeHelper.GetChild(parent, i);
        //        // If the child is not of the request child type child
        //        T childType = child as T;
        //        if (childType == null)
        //        {
        //            // recursively drill down the tree
        //            foundChild = FindChild<T>(child, childName);

        //            // If the child is found, break so we do not overwrite the found child. 
        //            if (foundChild != null) break;
        //        }
        //        else if (!string.IsNullOrEmpty(childName))
        //        {
        //            var frameworkElement = child as FrameworkElement;
        //            // If the child's name is set for search
        //            if (frameworkElement != null && frameworkElement.Name == childName)
        //            {
        //                // if the child's name is of the request name
        //                foundChild = (T)child;
        //                break;
        //            }
        //        }
        //        else
        //        {
        //            // child element found.
        //            foundChild = (T)child;
        //            break;
        //        }
        //    }

        //    return foundChild;
        //}

        public static List<T> FindChildren<T>(DependencyObject parent) where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            List<T> foundChild = new List<T>();

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T)
                    foundChild.Add((T)child);

                foundChild.AddRange(FindChildren<T>(child));
            }

            return foundChild;
        }

        public static T FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }

        public static T FindParent<T>(Control element) where T : Control
        {
            // Confirm parent and childName are valid. 
            if (element == null) return null;

            Control dobj = element.Parent as Control;

            if (dobj is T)
            {
                return (T)dobj;
            }
            else
            {
                return FindParent<T>(dobj);
            }
        }

        public static T FindVisualParent<T>(DependencyObject element) where T : Control
        {
            // Confirm parent and childName are valid. 
            if (element == null) return null;

            DependencyObject dobj = VisualTreeHelper.GetParent(element);

            if (dobj is T)
            {
                return (T)dobj;
            }
            else
            {
                return FindVisualParent<T>(dobj);
            }
        }

        public static List<T> FindVisualParents<T>(DependencyObject element) where T : Control
        {
            // Confirm parent and childName are valid. 
            if (element == null) return null;
            List<T> list = new List<T>();

            DependencyObject dobj = VisualTreeHelper.GetParent(element);
            if (dobj == null) return list;
            if (dobj is T)
            {
                list.Add((T)dobj);    
            }

            list.AddRange(FindVisualParents<T>(dobj));
            return list;
            
        }

        public static ListBoxItem GetListItem(ListBox listView_dinners, object item)
        {
            ListBoxItem lvi = listView_dinners.ItemContainerGenerator
                                   .ContainerFromItem(item) as ListBoxItem;
            return lvi;
        }

        public static void DispatcherIdleRun(DispatcherObject uc,Action act)
        {
            uc.Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(delegate()
            {
                act();
            }));
        }

    }
}
