import React, { useState } from 'react';
import axios from 'axios';
import "bootstrap/dist/css/bootstrap.min.css";
//import "../styles/PasswordReset.css";



const PasswordReset = () => {
    const [email, setEmail] = useState('');
    const [newPassword, setNewPassword] = useState('');
    const [token, setToken] = useState('');
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [message, setMessage] = useState('');
    const [error, setError] = useState('');
    const [isForgotPassword, setIsForgotPassword] = useState(true); // Toggle between forgot and reset
    const [isPasswordReset, setIsPasswordReset] = useState(false); // New state to track successful reset

    // Handle Forgot Password
    const handleForgotPassword = async (e) => {
        e.preventDefault();
        setIsSubmitting(true);
        setError('');
        setMessage('');

        try {
            const response = await axios.post('https://localhost:7256/api/Authentication/forgot-password', { email });
            setMessage(response.data.Message);
            setToken(response.data.token); // Store the token returned from the backend
            setIsForgotPassword(false); // Show reset password form after successful request
        } catch (error) {
            setError(error.response?.data?.Message || 'An error occurred while requesting the password reset.');
        } finally {
            setIsSubmitting(false);
        }
    };

    // Handle Reset Password
    const handleResetPassword = async (e) => {
        e.preventDefault();
        setIsSubmitting(true);
        setError('');
        setMessage('');

        try {
            const response = await axios.post('https://localhost:7256/api/Authentication/reset-password', { email, token, newPassword });
            setMessage(response.data.Message);
            setIsPasswordReset(true); // Set state to show the success page
            setTimeout(() => {
                window.location.href = '/login'; // Redirect to login after 2 seconds
            }, 2000);
        } catch (error) {
            setError(error.response?.data?.Message || 'An error occurred while resetting the password.');
        } finally {
            setIsSubmitting(false);
        }
    };

    return (
        <div className="auth-form-container d-flex justify-content-center align-items-center" style={{ backgroundColor: "#f7f7f7", height: "100vh", width: "100vw" }}>
            {isPasswordReset ? (
                <div className="text-center">
                    <h2>Password Reset Successfully</h2>
                    <div className="mb-3">
                        <i className="fas fa-check-circle" style={{ fontSize: '100px', color: 'green' }}></i>
                    </div>
                    <p>Your password has been successfully reset. You can now log in with your new password.</p>
                    <button
                        onClick={() => window.location.href = '/login'} // Redirect to login page
                        className="btn btn-success btn-block"
                    >
                        Go to Login
                    </button>
                </div>
            ) : (
                <form className="auth-form p-4 rounded shadow-lg" onSubmit={isForgotPassword ? handleForgotPassword : handleResetPassword} style={{ backgroundColor: "#ffffff", width: "100%", maxWidth: "500px" }}>
                    <h2 className="text-center mb-4">{isForgotPassword ? 'Forgot Password' : 'Reset Password'}</h2>

                    <div className="form-group">
                        <label htmlFor="email">Email</label>
                        <input
                            type="email"
                            id="email"
                            className="form-control"
                            placeholder="Enter your email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            required
                        />
                    </div>

                    {/* Show reset token input if it's the reset password form */}
                    {!isForgotPassword && (
                        <div className="form-group">
                            <label htmlFor="token">Reset Token</label>
                            <input
                                type="text"
                                id="token"
                                className="form-control"
                                placeholder="Token has been sent to your email"
                                value={token} // Automatically filled with the token
                                readOnly
                            />
                        </div>
                    )}

                    {/* Show new password input if it's the reset password form */}
                    {!isForgotPassword && (
                        <div className="form-group">
                            <label htmlFor="newPassword">New Password</label>
                            <input
                                type="password"
                                id="newPassword"
                                className="form-control"
                                placeholder="Enter your new password"
                                value={newPassword}
                                onChange={(e) => setNewPassword(e.target.value)}
                                required
                            />
                        </div>
                    )}

                    <button
                        type="submit"
                        className="btn btn-warning btn-block mt-3"
                        disabled={isSubmitting}
                    >
                        {isSubmitting ? 'Processing...' : isForgotPassword ? 'Send Reset Link' : 'Reset Password'}
                    </button>

                    {message && <div className="alert alert-success mt-3">{message}</div>}
                    {error && <div className="alert alert-danger mt-3">{error}</div>}
                </form>
            )}

            {/* Toggle between Forgot and Reset password */}
            {isForgotPassword ? (
                <p className="text-center">
                    Already have a token? <a href="#" onClick={() => setIsForgotPassword(false)}>Reset your password</a>
                </p>
            ) : (
                <p className="text-center">
                    Want to request a new reset link? <a href="#" onClick={() => setIsForgotPassword(true)}>Forgot password</a>
                </p>
            )}
        </div>
    );
};

export default PasswordReset;




