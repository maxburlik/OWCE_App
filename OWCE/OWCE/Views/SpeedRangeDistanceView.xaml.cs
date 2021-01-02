﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace OWCE.Views
{
    public partial class SpeedRangeDistanceView : ContentView
    {

        public static readonly BindableProperty RPMProperty = BindableProperty.Create(
            "RPM",
            typeof(int),
            typeof(SpeedRangeDistanceView));

        public int RPM
        {
            get { return (int)GetValue(RPMProperty); }
            set
            {
                SetValue(RPMProperty, value);
            }
        }

        public static readonly BindableProperty SpeedProperty = BindableProperty.Create(
            "Speed",
            typeof(int),
            typeof(SpeedRangeDistanceView));

        public int Speed
        {
            get { return (int)GetValue(SpeedProperty); }
            set
            {
                SetValue(SpeedProperty, value);
            }
        }

        public static readonly BindableProperty LifetimeOdometerProperty = BindableProperty.Create(
            "LifetimeOdometer",
            typeof(int),
            typeof(SpeedRangeDistanceView));

        public int LifetimeOdometer
        {
            get { return (int)GetValue(LifetimeOdometerProperty); }
            set { SetValue(LifetimeOdometerProperty, value); }
        }


        public static readonly BindableProperty TripOdometerProperty = BindableProperty.Create(
            "TripOdometer",
            typeof(int),
            typeof(SpeedRangeDistanceView));

        public int TripOdometer
        {
            get { return (int)GetValue(TripOdometerProperty); }
            set { SetValue(TripOdometerProperty, value); }
        }

        



        public SpeedRangeDistanceView()
        {
            InitializeComponent();
        }

        void MainArcView_SizeChanged(System.Object sender, System.EventArgs e)
        {
            if (MainArcView.Width > 0)
            {
                //GridThing.RowDefinitions[0].Height = MainArcView.Width * 0.523465704f;
            }
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == BindingContextProperty.PropertyName)
            {
                var bindingContext = BindingContext;
            }
        }

        void ExpanderView_PropertyChanged(System.Object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (Expander.IsExpandedProperty.PropertyName.Equals(e.PropertyName))
            {
                if (ExpanderView.IsExpanded)
                {
                    ExpanderArrow.RotateTo(180, ExpanderView.ExpandAnimationLength, ExpanderView.ExpandAnimationEasing);
                }
                else
                {
                    ExpanderArrow.RotateTo(0, ExpanderView.CollapseAnimationLength, ExpanderView.CollapseAnimationEasing);
                }
            }
        }
    }
}
