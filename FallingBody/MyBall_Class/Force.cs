using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ball_Class
{
    public class Force
    {
        public double forceX = 0;
        public double forceY = 0;

        public Force(double Fx, double Fy) {
            this.forceX = Fx;
            this.forceY = Fy;
        }
    }
}
