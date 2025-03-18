using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    public enum MoveState
    {
        LegalMove,
        IllegalMove
    }
    public class MoveIndicators : IDrawable
    {
        private HashSet<IShape> _indicators = new HashSet<IShape>();
        public HashSet<IShape> Indicators
        {
            get => _indicators;
            set => _indicators = value;
        }
        public void Add(IShape indicator)
        {
            _indicators.Add(indicator);
        }
        public void Clear()
        {
            _indicators.Clear();
        }

        public void Draw()
        {
            foreach (Shape indicator in Indicators)
            {
                indicator.Draw();
            }
        }

    }
}
