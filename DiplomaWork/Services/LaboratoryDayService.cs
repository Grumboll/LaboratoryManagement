using DiplomaWork.DataItems;
using DiplomaWork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Microsoft.EntityFrameworkCore;

namespace DiplomaWork.Services
{
    public class LaboratoryDayService
    {
        public static List<LaboratoryDayItem> getLaboratoryDayItems(laboratory_2023Context context, DateTime now, bool getForMonth = false)
        {
            List<LaboratoryDayItem> items = context.LaboratoryDays
                    .Where(ld => !getForMonth || ld.MonthId == now.Month)
                    .Where(ld => !getForMonth || ld.Year == now.Year)
                    .Where(ld => getForMonth || ld.Day == DateOnly.FromDateTime(now))
                    .Where(ld => ld.DeletedAt == null)
                    .Include(x => x.ProfileHasLengthsPerimeter)
                    .ThenInclude(x => x.Profile)
                    .Select(ld => new LaboratoryDayItem
                    {
                        Id = ld.Id,
                        ProfileHasLengthsPerimeterId = ld.ProfileHasLengthsPerimeterId,
                        Day = ld.Day.ToString(),
                        ProfileName = ld.ProfileHasLengthsPerimeter.Profile.Name,
                        ProfileLength = ld.ProfileHasLengthsPerimeter.Length.ToString(),
                        ProfilePerimeter = ld.ProfileHasLengthsPerimeter.Perimeter.ToString(),
                        MetersSquaredPerSample = ld.MetersSquaredPerSample.ToString(),
                        PaintedSamplesCount = ld.PaintedSamplesCount.ToString(),
                        PaintedMetersSquared = ld.PaintedMetersSquared.ToString(),
                        KilogramsPerMeter = ld.KilogramsPerMeter.ToString()
                    }).ToList();

            foreach (LaboratoryDayItem item in items)
            {
                item.ProfileLength = item.ProfileLength.TrimEnd('0').TrimEnd('.');
                item.ProfilePerimeter = item.ProfilePerimeter.TrimEnd('0').TrimEnd('.');
                item.MetersSquaredPerSample = item.MetersSquaredPerSample.TrimEnd('0').TrimEnd('.');
                item.PaintedMetersSquared = item.PaintedMetersSquared.TrimEnd('0').TrimEnd('.');
                item.KilogramsPerMeter = item.KilogramsPerMeter != null ? item.KilogramsPerMeter.TrimEnd('0').TrimEnd('.') : null;
            }

            return items;
        }
    }
}
