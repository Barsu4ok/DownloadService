using DownloadService.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadService.Validators
{
    public class CellInfoValidator : AbstractValidator<CellInfo>
    {
        public CellInfoValidator()
        {
            RuleFor(cellTower => cellTower.Radio).NotEmpty();
            RuleFor(cellTower => cellTower.MCC).NotEmpty().Must(isInteger);
            RuleFor(cellTower => cellTower.MNC).NotEmpty().Must(isInteger);
            RuleFor(cellTower => cellTower.LAC).NotEmpty().Must(isInteger);
            RuleFor(cellTower => cellTower.CID).NotEmpty().Must(isInteger);
            RuleFor(cellTower => cellTower.LON).NotEmpty().Must(isDouble);
            RuleFor(cellTower => cellTower.LAN).NotEmpty().Must(isDouble);
        }
        private bool isInteger(int value)
        {
            return int.TryParse(value.ToString(), out _);
        }
        private bool isDouble(double value)
        {
            return double.TryParse(value.ToString(), out _);
        }
    }
}
