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
                    .Join(context.Profiles,
                        phlp => phlp.ProfileId,
                        profile => profile.Id,
                        (phlp, profile) => new { phlp, profile })
                    .Select(p => new ProfileItem { Id = p.phlp.Id, ProfileName = p.profile.Name }).ToList();

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

