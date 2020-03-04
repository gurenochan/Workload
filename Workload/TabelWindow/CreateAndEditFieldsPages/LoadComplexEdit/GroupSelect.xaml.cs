using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Workload.TabelWindow.CreateAndEditFieldsPages.LoadComplexEdit
{
    /// <summary>
    /// Interaction logic for GroupSelect.xaml
    /// </summary>
    public partial class GroupSelect : Page
    {
        public GroupSelect()
        {
            InitializeComponent();
            this.GroupsInColl = new ObservableCollection<GROUPS_TBL>();
            this.GroupsOutColl = new ObservableCollection<GROUPS_TBL>();

            this.GroupsInColl.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler((object obj, System.Collections.Specialized.NotifyCollectionChangedEventArgs args) =>
            {
                foreach (GROUPS_TBL group in this.avGroups.Where(p => this.GroupsInColl.Any(g => g.GROUP_ID == p.GROUP_ID) && this.GroupsOutColl.Any(g => g.GROUP_ID == p.GROUP_ID))) this.GroupsOutColl.Remove(group);
                foreach (GROUPS_TBL group in this.avGroups.Where(p => !this.GroupsInColl.Any(g => g.GROUP_ID == p.GROUP_ID) && !this.GroupsOutColl.Any(g => g.GROUP_ID == p.GROUP_ID))) this.GroupsOutColl.Add(group);
                switch (args.Action)
                {
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                        foreach (GROUPS_TBL group in args.NewItems)
                        {
                            if (!this.subDetail.GROUPS_TBL.ToList().Any(p => p.GROUP_ID == group.GROUP_ID))
                                this.subDetail.GROUPS_TBL.Add(group);
                        }
                        break;
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                        foreach (GROUPS_TBL group in args.OldItems)
                        {
                            if (this.subDetail.GROUPS_TBL.ToList().Any(p => p.GROUP_ID == group.GROUP_ID))
                                this.subDetail.GROUPS_TBL.Remove(group);
                        }
                        break;
                }
            });

            this.GroupsInSubList.ItemsSource = this.GroupsInColl;
            this.GroupsOutSubList.ItemsSource = this.GroupsOutColl;

            this.AddGroupBut.Click += new RoutedEventHandler((object obj, RoutedEventArgs args) => this.AddGroup());
            this.RemoveGroupBut.Click += new RoutedEventHandler((object obj, RoutedEventArgs args) => this.RemoveGroup());

            this.GroupsOutSubList.MouseDoubleClick += new MouseButtonEventHandler((object sender, MouseButtonEventArgs args) =>
            {
                DependencyObject obj = (DependencyObject)args.OriginalSource;
                while (obj != null && obj != this.GroupsInSubList)
                {
                    if (obj.GetType() == typeof(ListViewItem))
                    {
                        this.AddGroup();
                        break;
                    }
                    obj = VisualTreeHelper.GetParent(obj);
                }
            });

            this.GroupsInSubList.MouseDoubleClick += new MouseButtonEventHandler((object sender, MouseButtonEventArgs args) => 
            {
                DependencyObject obj = (DependencyObject)args.OriginalSource;
                while (obj != null && obj != this.GroupsOutSubList)
                {
                    if (obj.GetType() == typeof(ListViewItem))
                    {
                        this.RemoveGroup();
                        break;
                    }
                    obj = VisualTreeHelper.GetParent(obj);
                }
            });
        }

        protected SUBDETAILS_TBL subDetail = null;
        public SUBDETAILS_TBL SubDetail
        {
            get => this.subDetail;
            set
            {
                this.subDetail = value;
                try
                {
                    this.GroupsInColl.Clear();
                }
                catch { }
                try
                { foreach (GROUPS_TBL group in value.GROUPS_TBL.ToList()) this.GroupsInColl.Add(group); }
                catch { }
            }
        }

        protected List<GROUPS_TBL> avGroups;
        public List<GROUPS_TBL> AvGroups
        {
            get => this.avGroups;
            set 
            {
                this.avGroups = value;
                try
                {
                    this.GroupsOutColl.Clear();
                }
                catch { }
                foreach (GROUPS_TBL group in value.Where(p => !(this.subDetail?.GROUPS_TBL ?? new List<GROUPS_TBL>()).Any(g => g.GROUP_ID == p.GROUP_ID))) this.GroupsOutColl.Add(group);
            }
        }

        public ObservableCollection<GROUPS_TBL>
            GroupsInColl,
            GroupsOutColl;

        public void AddGroup()
        {
            foreach (GROUPS_TBL group in GroupsOutSubList.SelectedItems.OfType<GROUPS_TBL>().ToList()) this.GroupsInColl.Add(group);
        }

        public void RemoveGroup()
        {
            foreach (GROUPS_TBL group in GroupsInSubList.SelectedItems.OfType<GROUPS_TBL>().ToList()) this.GroupsInColl.Remove(group);
        }
    }

}
