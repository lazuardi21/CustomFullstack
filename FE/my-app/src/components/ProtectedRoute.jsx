// components/ProtectedRoute.jsx
import React from 'react';
import { Route, Navigate } from 'react-router-dom';
import Cookies from 'js-cookie';

const ProtectedRoute = ({ element, ...rest }) => {
  const isAuthenticated = Cookies.get('token'); // Check if token exists in cookies or your preferred authentication logic

  return isAuthenticated ? (
    <Route {...rest} element={element} />
  ) : (
    <Navigate to="/login" replace />
  );
};

export default ProtectedRoute;
