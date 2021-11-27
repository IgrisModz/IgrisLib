using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace IgrisLib.Mvvm
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        Dictionary<string, object> _propertyBag;
#if NETFRAMEWORK || NETCOREAPP2_0
        Dictionary<string, object> PropertyBag => _propertyBag ?? (_propertyBag = new Dictionary<string, object>());
#else
        Dictionary<string, object> PropertyBag => _propertyBag ??= new Dictionary<string, object>();
#endif

        #region Events

        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void VerifyAccess()
        {
        }

        static bool CompareValues<T>(T storage, T value)
        {
            return EqualityComparer<T>.Default.Equals(storage, value);
        }

        static void GuardPropertyName(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException(nameof(propertyName));
        }

        [Conditional("DEBUG")]
        private void EnsureProperty([CallerMemberName] string propertyName = null)
        {
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                throw new ArgumentException("Property doesn't exist.", "propertyName");
            }
        }

        #region PropertyName Methods

        internal static string GetPropertyName(LambdaExpression lambdaExpression)
        {
            MemberExpression memberExpression;

            if (lambdaExpression.Body is UnaryExpression)
            {
                memberExpression = (lambdaExpression.Body as UnaryExpression).Operand as MemberExpression;
            }
            else
            {
                memberExpression = lambdaExpression.Body as MemberExpression;
            }

            return memberExpression.Member.Name;
        }

        internal static string GetPropertyNameFast(LambdaExpression expression)
        {
            if (!(expression.Body is MemberExpression memberExpression))
            {
                throw new ArgumentException("MemberExpression is expected in expression.Body", "expression");
            }
            const string vblocalPrefix = "$VB$Local_";
            MemberInfo member = memberExpression.Member;
            if (member.MemberType == MemberTypes.Field &&
                member.Name != null &&
                member.Name.StartsWith(vblocalPrefix))
#if NETCOREAPP3_0_OR_GREATER
                return member.Name[vblocalPrefix.Length..];
#else
                return member.Name.Substring(vblocalPrefix.Length);
#endif
            return member.Name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string GetPropertyName<T>(Expression<Func<T>> expression)
        {
            return GetPropertyNameFast(expression);
        }

        #endregion PropertyName Methods

        #region RaisePropertyChanged

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        protected void RaisePropertyChanged(string propertyName)
        {
            EnsureProperty(propertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 
        /// </summary>
        protected void RaisePropertyChanged()
        {
            RaisePropertiesChanged(null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyNames"></param>
        protected void RaisePropertiesChanged(params string[] propertyNames)
        {
            if (propertyNames == null || propertyNames.Length == 0)
            {
                RaisePropertyChanged(string.Empty);
                return;
            }
            foreach (string propertyName in propertyNames)
            {
                RaisePropertyChanged(propertyName);
            }
        }

        #endregion

        #region RaisePropertiesChanged

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        protected void RaisePropertyChanged<T>(Expression<Func<T>> expression)
        {
            RaisePropertyChanged(GetPropertyName(expression));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="expression1"></param>
        /// <param name="expression2"></param>
        protected void RaisePropertiesChanged<T1, T2>(Expression<Func<T1>> expression1, Expression<Func<T2>> expression2)
        {
            RaisePropertyChanged(expression1);
            RaisePropertyChanged(expression2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="expression1"></param>
        /// <param name="expression2"></param>
        /// <param name="expression3"></param>
        protected void RaisePropertiesChanged<T1, T2, T3>(Expression<Func<T1>> expression1, Expression<Func<T2>> expression2, Expression<Func<T3>> expression3)
        {
            RaisePropertyChanged(expression1);
            RaisePropertyChanged(expression2);
            RaisePropertyChanged(expression3);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <param name="expression1"></param>
        /// <param name="expression2"></param>
        /// <param name="expression3"></param>
        /// <param name="expression4"></param>
        protected void RaisePropertiesChanged<T1, T2, T3, T4>(Expression<Func<T1>> expression1, Expression<Func<T2>> expression2, Expression<Func<T3>> expression3, Expression<Func<T4>> expression4)
        {
            RaisePropertyChanged(expression1);
            RaisePropertyChanged(expression2);
            RaisePropertyChanged(expression3);
            RaisePropertyChanged(expression4);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <param name="expression1"></param>
        /// <param name="expression2"></param>
        /// <param name="expression3"></param>
        /// <param name="expression4"></param>
        /// <param name="expression5"></param>
        protected void RaisePropertiesChanged<T1, T2, T3, T4, T5>(Expression<Func<T1>> expression1, Expression<Func<T2>> expression2, Expression<Func<T3>> expression3, Expression<Func<T4>> expression4, Expression<Func<T5>> expression5)
        {
            RaisePropertyChanged(expression1);
            RaisePropertyChanged(expression2);
            RaisePropertyChanged(expression3);
            RaisePropertyChanged(expression4);
            RaisePropertyChanged(expression5);
        }

        #endregion RaisePropertiesChanged

        #region Property Methods

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected T GetProperty<T>(Expression<Func<T>> expression)
        {
            return GetPropertyCore<T>(GetPropertyName(expression));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storage"></param>
        /// <param name="value"></param>
        /// <param name="expression"></param>
        /// <param name="changedCallback"></param>
        /// <returns></returns>
        protected bool SetProperty<T>(ref T storage, T value, Expression<Func<T>> expression, Action changedCallback)
        {
            return SetProperty(ref storage, value, GetPropertyName(expression), changedCallback);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storage"></param>
        /// <param name="value"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected bool SetProperty<T>(ref T storage, T value, Expression<Func<T>> expression)
        {
            return SetProperty<T>(ref storage, value, expression, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected bool SetProperty<T>(Expression<Func<T>> expression, T value)
        {
            return SetProperty(expression, value, (Action)null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="value"></param>
        /// <param name="changedCallback"></param>
        /// <returns></returns>
        protected bool SetProperty<T>(Expression<Func<T>> expression, T value, Action changedCallback)
        {
            string propertyName = GetPropertyName(expression);
            return SetPropertyCore(propertyName, value, changedCallback);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="value"></param>
        /// <param name="changedCallback"></param>
        /// <returns></returns>
        protected bool SetProperty<T>(Expression<Func<T>> expression, T value, Action<T> changedCallback)
        {
            string propertyName = GetPropertyName(expression);
            return SetPropertyCore(propertyName, value, changedCallback);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storage"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <param name="changedCallback"></param>
        /// <returns></returns>
        protected virtual bool SetProperty<T>(ref T storage, T value, string propertyName, Action changedCallback)
        {
            VerifyAccess();
            if (CompareValues<T>(storage, value))
                return false;
            storage = value;
            RaisePropertyChanged(propertyName);
            changedCallback?.Invoke();
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storage"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected bool SetProperty<T>(ref T storage, T value, string propertyName)
        {
            return SetProperty(ref storage, value, propertyName, null);
        }

        #endregion Property Methods

        #region Value Methods

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected T GetValue<T>([CallerMemberName] string propertyName = null)
        {
            GuardPropertyName(propertyName);
            return GetPropertyCore<T>(propertyName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected bool SetValue<T>(T value, [CallerMemberName] string propertyName = null)
        {
            return SetValue(value, default(Action), propertyName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="changedCallback"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected bool SetValue<T>(T value, Action changedCallback, [CallerMemberName] string propertyName = null)
        {
            return SetPropertyCore(propertyName, value, changedCallback);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="changedCallback"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected bool SetValue<T>(T value, Action<T> changedCallback, [CallerMemberName] string propertyName = null)
        {
            return SetPropertyCore(propertyName, value, changedCallback);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storage"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected bool SetValue<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            return SetValue(ref storage, value, default, propertyName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storage"></param>
        /// <param name="value"></param>
        /// <param name="changedCallback"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected bool SetValue<T>(ref T storage, T value, Action changedCallback, [CallerMemberName] string propertyName = null)
        {
            GuardPropertyName(propertyName);
            return SetProperty(ref storage, value, propertyName, changedCallback);
        }

        #endregion Value Methods

        #region PropertyCore Methods
        T GetPropertyCore<T>(string propertyName)
        {
            if (PropertyBag.TryGetValue(propertyName, out object val))
                return (T)val;
            return default;
        }

        bool SetPropertyCore<T>(string propertyName, T value, Action changedCallback)
        {
            var res = SetPropertyCore(propertyName, value, out T oldValue);
            if (res)
            {
                changedCallback?.Invoke();
            }
            return res;
        }

        bool SetPropertyCore<T>(string propertyName, T value, Action<T> changedCallback)
        {
            var res = SetPropertyCore(propertyName, value, out T oldValue);
            if (res)
            {
                changedCallback?.Invoke(oldValue);
            }
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <param name="oldValue"></param>
        /// <returns></returns>
        protected virtual bool SetPropertyCore<T>(string propertyName, T value, out T oldValue)
        {
            VerifyAccess();
            oldValue = default;
            if (PropertyBag.TryGetValue(propertyName, out object val))
                oldValue = (T)val;
            if (CompareValues<T>(oldValue, value))
                return false;
            lock (PropertyBag)
            {
                PropertyBag[propertyName] = value;
            }
            RaisePropertyChanged(propertyName);
            return true;
        }
        #endregion
    }
}
