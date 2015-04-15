using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Views.InputMethods;
using System.Collections.Generic;
using System.Linq;

namespace XamarinListViewSortAndFilter {
    [Activity(Label = "XamarinListViewSortAndFilter", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity {
        private List<Friend> mFriends;
        private ListView mListView;
        private EditText mSearch;
        private LinearLayout mContainer;
        private bool mAnimatedDown;
        private bool mIsAnimating;
        private FriendsAdapter mAdapter;

        private TextView txtFistsName;
        private TextView txtLastName;
        private TextView txtAge;
        private TextView txtGender;

        protected override void OnCreate(Bundle bundle) {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            mListView = FindViewById<ListView>(Resource.Id.listView);
            mSearch = FindViewById<EditText>(Resource.Id.etSearch);
            mContainer = FindViewById<LinearLayout>(Resource.Id.llContainer);

            txtFistsName = FindViewById<TextView>(Resource.Id.txtFistsName);
            txtLastName = FindViewById<TextView>(Resource.Id.txtLastName);
            txtAge = FindViewById<TextView>(Resource.Id.txtAge);
            txtGender = FindViewById<TextView>(Resource.Id.txtGender);

            txtFistsName.Tag = false;
            txtLastName.Tag = false;
            txtAge.Tag = false;
            txtGender.Tag = false;

            txtFistsName.Click += txtFistsName_Click;
            txtLastName.Click += txtLastName_Click;
            txtAge.Click += txtAge_Click;
            txtGender.Click += txtGender_Click;

            mSearch.Alpha = 0;
            mContainer.BringToFront();

            mSearch.TextChanged += mSearch_TextChanged;

            mFriends = new List<Friend>();
            mFriends.Add(new Friend { FirstName = "Bob", LastName = "Smith", Age = "33", Gender = "Male" });
            mFriends.Add(new Friend { FirstName = "Tom", LastName = "Smith", Age = "45", Gender = "Male" });
            mFriends.Add(new Friend { FirstName = "Julie", LastName = "Smith", Age = "2020", Gender = "Unknown" });
            mFriends.Add(new Friend { FirstName = "Molly", LastName = "Smith", Age = "21", Gender = "Female" });
            mFriends.Add(new Friend { FirstName = "Joe", LastName = "Lopez", Age = "22", Gender = "Male" });
            mFriends.Add(new Friend { FirstName = "Ruth", LastName = "White", Age = "81", Gender = "Female" });
            mFriends.Add(new Friend { FirstName = "Sally", LastName = "Johnson", Age = "54", Gender = "Female" });

            mAdapter = new FriendsAdapter(this, Resource.Layout.row_friend, mFriends);
            mListView.Adapter = mAdapter;
        }

        void txtGender_Click(object sender, EventArgs e) {
            var list = mFriends.OrderBy(x => x.Gender).ToList();

            if ((bool)txtGender.Tag)
                list.Reverse();

            txtGender.Tag = !(bool)txtGender.Tag;

            mAdapter = new FriendsAdapter(this, Resource.Layout.row_friend, list);
            mListView.Adapter = mAdapter;
        }

        void txtAge_Click(object sender, EventArgs e) {
            var list = mFriends.OrderBy(x => x.Age).ToList();

            if ((bool)txtAge.Tag)
                list.Reverse();

            txtAge.Tag = !(bool)txtAge.Tag;

            mAdapter = new FriendsAdapter(this, Resource.Layout.row_friend, list);
            mListView.Adapter = mAdapter;
        }

        void txtLastName_Click(object sender, EventArgs e) {
            var list = mFriends.OrderBy(x => x.LastName).ToList();

            if ((bool)txtLastName.Tag)
                list.Reverse();

            txtLastName.Tag = !(bool)txtLastName.Tag;

            mAdapter = new FriendsAdapter(this, Resource.Layout.row_friend, list);
            mListView.Adapter = mAdapter;
        }

        void txtFistsName_Click(object sender, EventArgs e) {
            var list = mFriends.OrderBy(x => x.FirstName).ToList();

            if ((bool)txtFistsName.Tag)
                list.Reverse();

            txtFistsName.Tag = !(bool)txtFistsName.Tag;

            mAdapter = new FriendsAdapter(this, Resource.Layout.row_friend, list);
            mListView.Adapter = mAdapter;
        }

        void mSearch_TextChanged(object sender, Android.Text.TextChangedEventArgs e) {
            List<Friend> searchedFriends = (
                                    from friend in mFriends
                                            where 
                                                friend.FirstName.Contains(mSearch.Text, StringComparison.OrdinalIgnoreCase) 
                                                || 
                                                friend.LastName.Contains(mSearch.Text, StringComparison.OrdinalIgnoreCase)
                                                || 
                                                friend.Age.Contains(mSearch.Text, StringComparison.OrdinalIgnoreCase) 
                                                || 
                                                friend.Gender.Contains(mSearch.Text, StringComparison.OrdinalIgnoreCase)
                                            select friend
                            ).ToList<Friend>();

            //Refreshes the listview
            mAdapter = new FriendsAdapter(this, Resource.Layout.row_friend, searchedFriends);
            mListView.Adapter = mAdapter;
        }

        public override bool OnCreateOptionsMenu(IMenu menu) {
            MenuInflater.Inflate(Resource.Menu.actionbar, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item) {
            switch (item.ItemId) {

                case Resource.Id.search:
                    //Search icon has been clicked

                    if (mIsAnimating) {
                        return true;
                    }

                    if (!mAnimatedDown) {
                        //Listview is up
                        MyAnimation anim = new MyAnimation(mListView, mListView.Height - mSearch.Height);
                        anim.Duration = 500;
                        mListView.StartAnimation(anim);
                        anim.AnimationStart += anim_AnimationStartDown;
                        anim.AnimationEnd += anim_AnimationEndDown;
                        mContainer.Animate().TranslationYBy(mSearch.Height).SetDuration(500);
                    } else {
                        //Listview is down
                        MyAnimation anim = new MyAnimation(mListView, mListView.Height + mSearch.Height);
                        anim.Duration = 500;
                        mListView.StartAnimation(anim);
                        anim.AnimationStart += anim_AnimationStartUp;
                        anim.AnimationEnd += anim_AnimationEndUp;
                        mContainer.Animate().TranslationYBy(-mSearch.Height).SetDuration(500);
                    }

                    mAnimatedDown = !mAnimatedDown;
                    return true;

                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        void anim_AnimationEndUp(object sender, Android.Views.Animations.Animation.AnimationEndEventArgs e) {
            mIsAnimating = false;
            mSearch.ClearFocus();
            InputMethodManager inputManager = (InputMethodManager)this.GetSystemService(Context.InputMethodService);
            inputManager.HideSoftInputFromWindow(this.CurrentFocus.WindowToken, HideSoftInputFlags.NotAlways);
        }

        void anim_AnimationEndDown(object sender, Android.Views.Animations.Animation.AnimationEndEventArgs e) {
            mIsAnimating = false;
        }

        void anim_AnimationStartDown(object sender, Android.Views.Animations.Animation.AnimationStartEventArgs e) {
            mIsAnimating = true;
            mSearch.Animate().AlphaBy(1.0f).SetDuration(500);
        }

        void anim_AnimationStartUp(object sender, Android.Views.Animations.Animation.AnimationStartEventArgs e) {
            mIsAnimating = true;
            mSearch.Animate().AlphaBy(-1.0f).SetDuration(300);
        }
    }
}

