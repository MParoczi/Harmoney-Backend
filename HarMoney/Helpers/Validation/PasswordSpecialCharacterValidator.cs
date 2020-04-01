using System.Linq;


namespace HarMoney.Helpers.Validation
{
    public class PasswordSpecialCharacterValidator
    {
        private readonly char[] _forbiddenChars = new char[]
            {'"', '˘', '°', '˛', '`', '˙', '´', '˝', '¨', '¤', ' '};
        private readonly char[] _mandatoryChars = new char[]
            {'#', '?', '!', '@', '$', '%', '^', '&', '*', '_', '-'};
        public string ErrorMessage { get; private set; }

        public bool IsOk(string password)
        {
            if (MissingMandatoryChar(password))
            {
                ErrorMessage = "The password should contain at least one of the following special characters: " +
                               "#?!@$%^&*_-";
                return false;
            }

            if (password.IndexOfAny(_forbiddenChars) > 0)
            {
                ErrorMessage = "The password can't contain any of the following characters: °˛`˙´˝¨¤ or space.";
                return false;
            }

            return true;
        }

        private bool MissingMandatoryChar(string password)
        {
            for (int i = 0; i < password.Length; i++)
            {
                char letter = password[i];
                if (_mandatoryChars.Contains(letter))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
