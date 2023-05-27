using DiplomaWork.Models;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomaWork.DataItems
{
    public class ViewModel
    {
        public ObservableCollection<ProfileItem> Items { get; set; }

        private int _ProfileHasLengthPerimeterId;
        public int ProfileHasLengthsPerimeterId
        {
            get { return _ProfileHasLengthPerimeterId; }
            set
            {
                _ProfileHasLengthPerimeterId = value;
                OnPropertyChanged(nameof(ProfileHasLengthsPerimeterId));
            }
        }

        public ViewModel()
        {
            LoadData();
        }

        private void LoadData()
        {
            using (var context = new laboratory_2023Context())
            {
                var profiles = context.ProfileHasLengthsPerimeters
                    .Where(p => p.DeletedAt == null)
                    .Select(p => new ProfileItem { Id = p.Id, ProfileName = p.Profile.Name }).ToList();

                Items = new ObservableCollection<ProfileItem>(profiles);
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

