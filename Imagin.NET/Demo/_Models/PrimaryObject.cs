﻿using Imagin.Common;

namespace Imagin.NET.Demo
{
    public class PrimaryObject : NamedObject
    {
        bool boolean = false;
        public bool Boolean
        {
            get
            {
                return boolean;
            }
            set
            {
                boolean = value;
                OnPropertyChanged("Boolean");
            }
        }

        [Featured(true)]
        public override string Name
        {
            get => base.Name;
            set => base.Name = value;
        }

        double _double = 0d;
        public double Double
        {
            get => _double;
            set => Property.Set(this, ref _double, value, () => Double);
        }

        SecondaryObject child = new SecondaryObject();
        public SecondaryObject Child
        {
            get => child;
            set => Property.Set(this, ref child, value, () => Child);
        }

        public PrimaryObject() : base()
        {
        }
    }
}
