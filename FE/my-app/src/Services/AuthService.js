// // src/Services/AuthService.js

// import Cookies from 'js-cookie';

// export const login = async (email, password) => {
//   try {
//     const response = await fetch('http://localhost:5240/api/v1/Users/login', {
//       method: 'POST',
//       headers: {
//         'Content-Type': 'application/json',
//       },
//       body: JSON.stringify({ email, password }),
//     });

//     if (!response.ok) {
//       throw new Error('Login failed');
//     }

//     const data = await response.json();
//     Cookies.set('token', data.token, { expires: 7 }); // Store token in cookie with expiration (7 days)

//     return data;
//   } catch (error) {
//     throw new Error('Login failed');
//   }
// };

// export const logout = () => {
//   Cookies.remove('token'); // Remove token from cookie on logout
// };

// export const getToken = () => {
//   return Cookies.get('token'); // Retrieve token from cookie
// };
