using System;

namespace ParkingSystem.Common.Security
{
    public class ReadOnceSecret : IDisposable
    {
        private char[]? _secret;
        private bool _read;

        public ReadOnceSecret(string input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            _secret = input.ToCharArray();
            _read = false;
        }

        public string Read()
        {
            if (_read)
                throw new InvalidOperationException("Secret has already been read and is no longer accessible.");

            _read = true;
            string result = new string(_secret);
            Clear();
            return result;
        }

        private void Clear()
        {
            if (_secret != null)
            {
                for (int i = 0; i < _secret.Length; i++)
                    _secret[i] = '\0';

                _secret = null;
            }
        }

        public void Dispose()
        {
            Clear();
            GC.SuppressFinalize(this);
        }

        ~ReadOnceSecret()
        {
            Clear();
        }

        public override string ToString() => "<ReadOnceSecret>";
    }

}
