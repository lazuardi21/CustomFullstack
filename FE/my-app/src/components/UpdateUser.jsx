import React, { useState, useEffect } from 'react';
import Cookies from 'js-cookie';

const UpdateUser = () => {
  const [users, setUsers] = useState([]);
  const [selectedUser, setSelectedUser] = useState(null);
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [email, setEmail] = useState('');
  const [updateMessage, setUpdateMessage] = useState('');
  const [error, setError] = useState('');

  useEffect(() => {
    fetchUsers();
  }, []);

  const fetchUsers = async () => {
    const token = Cookies.get('token');
    try {
      const response = await fetch('http://localhost:5240/api/v1/Users', {
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });
      if (!response.ok) {
        throw new Error('Failed to fetch users');
      }
      const data = await response.json();
      setUsers(data);
    } catch (error) {
      console.error('Failed to fetch users:', error);
    }
  };

  const handleUserChange = async (userId) => {
    const token = Cookies.get('token');
    try {
      const response = await fetch(`http://localhost:5240/api/v1/Users/${userId}`, {
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });
      if (!response.ok) {
        throw new Error('Failed to fetch user details');
      }
      const data = await response.json();
      setSelectedUser(data);
      setFirstName(data.firstName);
      setLastName(data.lastName);
      setEmail(data.email);
    } catch (error) {
      console.error('Failed to fetch user details:', error);
      setSelectedUser(null);
      setError('Failed to fetch user details');
    }
  };

  const handleUpdate = async (event) => {
    event.preventDefault();
    const token = Cookies.get('token');
    try {
      const response = await fetch(`http://localhost:5240/api/v1/Users/${selectedUser.id}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify({
          firstName,
          lastName,
          email
        })
      });

      if (response.ok) {
        setUpdateMessage('User updated successfully');
        fetchUsers(); // Refresh the user list after update
      } else {
        setUpdateMessage('Failed to update user');
      }
    } catch (error) {
      console.error('Failed to update user:', error);
      setUpdateMessage('Failed to update user');
    }
  };

  return (
    <div className="max-w-md mx-auto mt-10">
      <h2 className="text-2xl font-bold mb-6">Update User</h2>
      <form onSubmit={handleUpdate}>
        <div className="mb-4">
          <label className="block text-sm font-semibold mb-1">User ID</label>
          <select
            onChange={(e) => handleUserChange(e.target.value)}
            className="w-full px-4 py-2 border rounded"
          >
            <option value="">Select User</option>
            {users.map((user) => (
              <option key={user.id} value={user.id}>
                {user.id} - {user.firstName} {user.lastName}
              </option>
            ))}
          </select>
        </div>
        {selectedUser && (
          <>
            <div className="mb-4">
              <label className="block text-sm font-semibold mb-1">First Name</label>
              <input
                type="text"
                value={firstName}
                onChange={(e) => setFirstName(e.target.value)}
                className="w-full px-4 py-2 border rounded"
              />
            </div>
            <div className="mb-4">
              <label className="block text-sm font-semibold mb-1">Last Name</label>
              <input
                type="text"
                value={lastName}
                onChange={(e) => setLastName(e.target.value)}
                className="w-full px-4 py-2 border rounded"
              />
            </div>
            <div className="mb-4">
              <label className="block text-sm font-semibold mb-1">Email</label>
              <input
                type="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                className="w-full px-4 py-2 border rounded"
              />
            </div>
            <button
              type="submit"
              className="w-full bg-blue-600 text-white py-2 rounded mt-4"
            >
              Update User
            </button>
          </>
        )}
      </form>
      {error && <p className="text-red-500 text-xs mt-1">{error}</p>}
      {updateMessage && <p className="text-green-500 text-xs mt-1">{updateMessage}</p>}
    </div>
  );
};

export default UpdateUser;
