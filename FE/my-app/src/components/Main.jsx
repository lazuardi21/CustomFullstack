import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import Cookies from 'js-cookie';
import UserTable from './UserTable'; // Import UserTable component
import CreateUser from './CreateUser'; // Import CreateUser component
import UpdateUser from './UpdateUser'; // Import UpdateUser component
import DeleteUser from './DeleteUser'; // Import DeleteUser component

const Main = () => {
  const navigate = useNavigate();
  const [expandedMenu, setExpandedMenu] = useState('');
  const [showAllUsers, setShowAllUsers] = useState(false);
  const [showCreateUser, setShowCreateUser] = useState(false);
  const [showUpdateUser, setShowUpdateUser] = useState(false);
  const [showDeleteUser, setShowDeleteUser] = useState(false); // New state for DeleteUser

  const handleLogout = () => {
    Cookies.remove('token');
    navigate('/login');
  };

  const handleMenuClick = (menu) => {
    if (expandedMenu === menu) {
      setExpandedMenu('');
    } else {
      setExpandedMenu(menu);
    }

    if (menu === 'user') {
      setShowAllUsers(false);
      setShowCreateUser(false);
      setShowUpdateUser(false);
      setShowDeleteUser(false); // Ensure DeleteUser is hidden when switching menus
    }
  };

  const handleShowAllUsers = () => {
    setShowAllUsers(true);
    setShowCreateUser(false);
    setShowUpdateUser(false);
    setShowDeleteUser(false);
  };

  const handleCreateUser = () => {
    setShowAllUsers(false);
    setShowCreateUser(true);
    setShowUpdateUser(false);
    setShowDeleteUser(false);
  };

  const handleUpdateUser = () => {
    setShowAllUsers(false);
    setShowCreateUser(false);
    setShowUpdateUser(true);
    setShowDeleteUser(false);
  };

  const handleDeleteUser = () => {
    setShowAllUsers(false);
    setShowCreateUser(false);
    setShowUpdateUser(false);
    setShowDeleteUser(true);
  };

  return (
    <div className="flex h-screen">
      {/* Sidebar */}
      <div className="bg-gray-800 text-white w-64">
        {/* Sidebar Content */}
        <div className="py-4 px-6">
          {/* Sidebar Header */}
          <div className="flex items-center justify-between">
            <div className="text-xl font-semibold">Grasfam</div>
            <img src="/path/to/dummy-image.png" alt="User" className="w-8 h-8 rounded-full" />
          </div>
          {/* Sidebar Menu */}
          <ul className="mt-6">
            {/* User Menu */}
            <li>
              <button
                className="flex items-center justify-between w-full py-2 px-4 text-sm font-medium hover:bg-gray-700"
                onClick={() => handleMenuClick('user')}
              >
                User
                <svg
                  className="h-4 w-4"
                  viewBox="0 0 20 20"
                  fill="currentColor"
                  xmlns="http://www.w3.org/2000/svg"
                >
                  {expandedMenu === 'user' ? (
                    <path
                      fillRule="evenodd"
                      clipRule="evenodd"
                      d="M5.293 7.293a1 1 0 0 1 1.414 0L10 10.586l3.293-3.293a1 1 0 1 1 1.414 1.414l-4 4a1 1 0 0 1-1.414 0l-4-4a1 1 0 0 1 0-1.414z"
                    />
                  ) : (
                    <path
                      fillRule="evenodd"
                      clipRule="evenodd"
                      d="M10 8a1 1 0 0 1 1 1v5a1 1 0 0 1-2 0V9a1 1 0 0 1 1-1zM4 6a1 1 0 1 1 0-2h12a1 1 0 1 1 0 2H4z"
                    />
                  )}
                </svg>
              </button>
              {expandedMenu === 'user' && (
                <ul className="pl-8 mt-2">
                  <li
                    className="py-2 hover:bg-gray-700 cursor-pointer"
                    onClick={handleShowAllUsers}
                  >
                    All Users
                  </li>
                  <li
                    className="py-2 hover:bg-gray-700 cursor-pointer"
                    onClick={handleCreateUser}
                  >
                    Create User
                  </li>
                  <li
                    className="py-2 hover:bg-gray-700 cursor-pointer"
                    onClick={handleUpdateUser}
                  >
                    Update User
                  </li>
                  <li
                    className="py-2 hover:bg-gray-700 cursor-pointer"
                    onClick={handleDeleteUser} // Handle Delete User click
                  >
                    Delete User
                  </li>
                </ul>
              )}
            </li>
            {/* Organizer Menu */}
            {/* SportEvent Menu */}
          </ul>
          {/* Logout Button */}
          <button
            className="block w-full py-2 px-4 mt-4 text-sm font-medium bg-red-600 hover:bg-red-700"
            onClick={handleLogout}
          >
            Logout
          </button>
        </div>
      </div>
      {/* Main Content */}
      <div className="flex-1 p-4">
        {/* Main Content Area */}
        {showAllUsers && <UserTable />} {/* Render UserTable component */}
        {showCreateUser && <CreateUser />} {/* Render CreateUser component */}
        {showUpdateUser && <UpdateUser />} {/* Render UpdateUser component */}
        {showDeleteUser && <DeleteUser />} {/* Render DeleteUser component */}
      </div>
    </div>
  );
};

export default Main;
