// src/Components/Login.jsx
import React, { useState, useRef } from 'react';
import { AiOutlineTwitter } from 'react-icons/ai';
import { BiLogoFacebook } from 'react-icons/bi';
import { Navigate, useNavigate } from 'react-router-dom'; // Import Navigate from react-router-dom
import { login } from '../Services/Api'; // Adjust the path as per your actual file structure
import './Login.css'; // Import CSS for animations

const Login = () => {
  const [enteredEmail, setEnteredEmail] = useState('');
  const [enteredPassword, setEnteredPassword] = useState('');
  const [submitted, setSubmitted] = useState(false);
  const [loginError, setLoginError] = useState('');
  const [loginSuccess, setLoginSuccess] = useState('');

  const navigate = useNavigate();

  const errorRef = useRef(null); // Ref for error message animation

  const handleInputChange = (identifier, value) => {
    if (identifier === 'email') {
      setEnteredEmail(value);
    } else {
      setEnteredPassword(value);
    }
  };

  const handleSubmit = async (event) => {
    event.preventDefault();
    setSubmitted(true);

    try {
      const data = await login(enteredEmail, enteredPassword);
      setLoginSuccess('Login successful');
      setLoginError('');

      // Redirect to '/dashboard' after successful login
      navigate('/Dashboard');
      
      console.log('Login successful:', data);
      // Handle successful login (e.g., store token, redirect)
    } catch (error) {
      setLoginSuccess('');
      setLoginError('Login failed. Please check your credentials.');
      console.error('Login failed:', error.message);
      // Handle login failure (e.g., show error message)
      
      // Trigger animation on error message
      if (errorRef.current) {
        errorRef.current.classList.add('shake-animation');
        setTimeout(() => {
          errorRef.current.classList.remove('shake-animation');
        }, 1000); // Remove animation class after 1 second
      }
    }
  };

  return (
    <section className="h-screen flex flex-col md:flex-row justify-center space-y-10 md:space-y-0 md:space-x-16 items-center my-2 mx-5 md:mx-0 md:my-0">
      <div className="md:w-1/3 max-w-sm">
        <img
          src="https://tecdn.b-cdn.net/img/Photos/new-templates/bootstrap-login-form/draw2.webp"
          alt="Sample image"
        />
      </div>
      <div className="md:w-1/3 max-w-sm">
        <div className="text-center md:text-left">
          <label className="mr-1">Sign in with</label>
          <button
            type="button"
            className="mx-1 h-9 w-9  rounded-full bg-blue-600 hover:bg-blue-700 text-white shadow-[0_4px_9px_-4px_#3b71ca]"
          >
            <BiLogoFacebook
              size={20}
              className="flex justify-center items-center w-full"
            />
          </button>
          <button
            type="button"
            className="inlne-block mx-1 h-9 w-9 rounded-full bg-blue-600 hover:bg-blue-700 uppercase leading-normal text-white shadow-[0_4px_9px_-4px_#3b71ca]"
          >
            <AiOutlineTwitter
              size={20}
              className="flex justify-center items-center w-full"
            />
          </button>
        </div>
        <div className="my-5 flex items-center before:mt-0.5 before:flex-1 before:border-t before:border-neutral-300 after:mt-0.5 after:flex-1 after:border-t after:border-neutral-300">
          <p className="mx-4 mb-0 text-center font-semibold text-slate-500">
            Or
          </p>
        </div>
        <form onSubmit={handleSubmit}>
          <input
            className={`text-sm w-full px-4 py-2 border border-solid border-gray-300 rounded ${submitted && !enteredEmail.includes('@') ? 'border-red-500' : ''}`}
            type="text"
            placeholder="Email Address"
            value={enteredEmail}
            onChange={(event) => handleInputChange('email', event.target.value)}
          />
          {submitted && !enteredEmail.includes('@') && (
            <p className="text-red-500 text-xs mt-1">Please enter a valid email address</p>
          )}
          <input
            className={`text-sm w-full px-4 py-2 border border-solid border-gray-300 rounded mt-4 ${submitted && enteredPassword.trim().length < 6 ? 'border-red-500' : ''}`}
            type="password"
            placeholder="Password"
            value={enteredPassword}
            onChange={(event) => handleInputChange('password', event.target.value)}
          />
          {submitted && enteredPassword.trim().length < 6 && (
            <p className="text-red-500 text-xs mt-1">Password must be at least 6 characters</p>
          )}
          <div className="mt-4 flex justify-between font-semibold text-sm">
            <label className="flex text-slate-500 hover:text-slate-600 cursor-pointer">
              <input className="mr-1" type="checkbox" />
              <span>Remember Me</span>
            </label>
            <a
              className="text-blue-600 hover:text-blue-700 hover:underline hover:underline-offset-4"
              href="#"
            >
              Forgot Password?
            </a>
          </div>
          <div className="text-center md:text-left">
            <button
              className="mt-4 bg-blue-600 hover:bg-blue-700 px-4 py-2 text-white uppercase rounded text-xs tracking-wider"
              type="submit"
            >
              Login
            </button>
          </div>
        </form>
        <div ref={errorRef} className="mt-4 font-semibold text-sm text-slate-500 text-center md:text-left">
          {loginError && <p className="text-red-500">{loginError}</p>}
          {loginSuccess && <p className="text-green-500">{loginSuccess}</p>}
        </div>
      </div>
    </section>
  );
};

export default Login;
