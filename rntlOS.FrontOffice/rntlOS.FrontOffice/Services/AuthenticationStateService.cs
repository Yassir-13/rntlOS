using rntlOS.Core.Models;

namespace rntlOS.FrontOffice.Services
{
    public class AuthenticationStateService
    {
        public Client? CurrentUser { get; private set; }
        public bool IsAuthenticated => CurrentUser != null;

        public event Action? OnAuthStateChanged;

        public void Login(Client client)
        {
            CurrentUser = client;
            NotifyAuthStateChanged();
        }

        public void Logout()
        {
            CurrentUser = null;
            NotifyAuthStateChanged();
        }

        private void NotifyAuthStateChanged()
        {
            OnAuthStateChanged?.Invoke();
        }
    }
}
