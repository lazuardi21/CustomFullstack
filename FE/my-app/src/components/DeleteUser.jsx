import React, { useState, useEffect } from 'react';
import Cookies from 'js-cookie';

const DeleteUser = () => {
  const [users, setUsers] = useState([]);
  const [selectedUserId, setSelectedUserId] = useState('');
  const [deleteMessage, setDeleteMessage] = useState('');
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

  const handleUserDelete = async () => {
    const token = Cookies.get('token');
    try {
      const response = await fetch(`http://localhost:5240/api/v1/Users/${selectedUserId}`, {
        method: 'DELETE',
        headers: {
          'Authorization': `Bearer ${token}`,
          'accept': '*/*'
        }
      });

      if (response.ok) {
        setDeleteMessage('User deleted successfully');
        fetchUsers(); // Refresh the user list after deletion
      } else {
        setDeleteMessage('Failed to delete user');
      }
    } catch (error) {
      console.error('Failed to delete user:', error);
      setDeleteMessage('Failed to delete user');
    }
  };

  return (
    <div className="max-w-md mx-auto mt-10">
      <h2 className="text-2xl font-bold mb-6">Delete User</h2>
      <div className="mb-4">
        <label className="block text-sm font-semibold mb-1">Select User to Delete</label>
        <select
          value={selectedUserId}
          onChange={(e) => setSelectedUserId(e.target.value)}
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
      {selectedUserId && (
        <button
          onClick={handleUserDelete}
          className="w-full bg-red-600 text-white py-2 rounded mt-4"
        >
          Delete User
        </button>
      )}
      {deleteMessage && <p className="text-green-500 text-xs mt-1">{deleteMessage}</p>}
      {error && <p className="text-red-500 text-xs mt-1">{error}</p>}
    </div>
  );
};

export default DeleteUser;
