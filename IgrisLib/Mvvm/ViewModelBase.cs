using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace IgrisLib.Mvvm
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        private readonly Dictionary<string, object> _propertyValueStorage;

        public event PropertyChangedEventHandler PropertyChanged;

        protected ViewModelBase()
        {
            this._propertyValueStorage = new Dictionary<string, object>();
        }

        ~ViewModelBase()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return false;
            }
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.EnsureProperty(propertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged<T>(Expression<Func<T>> selectorExpression)
        {
            if (selectorExpression == null)
                throw new ArgumentNullException("selectorExpression");
            if (!(selectorExpression.Body is MemberExpression body))
                throw new ArgumentException("The body must be a member expression");
            OnPropertyChanged(body.Member.Name);
        }

        protected bool SetField<T>(ref T field, T value, Expression<Func<T>> selectorExpression)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
            OnPropertyChanged(selectorExpression);
            return true;
        }


        [Conditional("DEBUG")]
        private void EnsureProperty(string propertyName)
        {
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                throw new ArgumentException("Property doesn't exist.", "propertyName");
            }
        }

        protected void SetValue<T>(Expression<Func<T>> property, T value)
        {
            if (!(property is LambdaExpression lambdaExpression))
            {
                throw new ArgumentException("Invalid lambda expression", "Lambda expression return value can't be null");
            }

            var propertyName = this.getPropertyName(lambdaExpression);
            var storedValue = this.getValue<T>(propertyName);

            if (object.Equals(storedValue, value)) return;

            this._propertyValueStorage[propertyName] = value;
            this.OnPropertyChanged(propertyName);
        }

        protected T GetValue<T>(Expression<Func<T>> property)
        {
            if (!(property is LambdaExpression lambdaExpression))
            {
                throw new ArgumentException("Invalid lambda expression", "Lambda expression return value can't be null");
            }

            var propertyName = this.getPropertyName(lambdaExpression);
            return getValue<T>(propertyName);
        }

        private T getValue<T>(string propertyName)
        {
            if (_propertyValueStorage.TryGetValue(propertyName, out object value))
            {
                return (T)value;
            }
            return default;

        }

        private string getPropertyName(LambdaExpression lambdaExpression)
        {
            MemberExpression memberExpression;

            if (lambdaExpression.Body is UnaryExpression)
            {
                var unaryExpression = lambdaExpression.Body as UnaryExpression;
                memberExpression = unaryExpression.Operand as MemberExpression;
            }
            else
            {
                memberExpression = lambdaExpression.Body as MemberExpression;
            }

            return memberExpression.Member.Name;
        }
    }
}
