# Authentication Setup

The authentication system has been configured with the following components:

1. **Cookie Authentication** - Users are authenticated using cookie-based authentication
2. **Login/Logout Process**
   - Login at `/Account/Login`
   - Logout at `/Account/Logout`
   - Default credentials: username=admin, password=password

3. **Protected Routes**
   - The home page (`/Home/Index`) requires authentication
   - Unauthenticated users are redirected to the login page

4. **UI Components**
   - Login link in navigation for unauthenticated users
   - User name display and logout button for authenticated users

5. **Login Form Features**
   - Username and password validation
   - Return URL support for redirect after login
   - Error message display for invalid credentials

To test the authentication:
1. Try accessing the home page - you'll be redirected to login
2. Log in with username "admin" and password "password"
3. After successful login you'll be redirected back to the home page
4. Use the logout button to end your session