// App.jsx
import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import Cookies from 'js-cookie';
import Login from './components/Login';
import Main from './components/Main';
import Dashboard from './components/Dashboard';

const App = () => {
  const isAuthenticated = !!Cookies.get('token'); // Check if token exists in cookies

  return (
    <Router>
      <Routes>
        {/* Route for "/login" */}
        <Route path="/login" element={<Login />} />

        {/* Protected Route for "/dashboard" */}
        <Route
          path="/dashboard/*"
          element={isAuthenticated ? <Main /> : <Navigate to="/login" replace />}
        />

        {/* Default route */}
        <Route
          path="*"
          element={
            isAuthenticated ? (
              <Navigate to="/dashboard" replace />
            ) : (
              <Navigate to="/login" replace />
            )
          }
        />
      </Routes>
    </Router>
  );
};

export default App;
