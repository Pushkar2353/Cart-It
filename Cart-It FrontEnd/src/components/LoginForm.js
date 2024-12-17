import React, { useState } from "react";
import axios from "axios";
import PasswordReset from './PasswordReset'; // Import PasswordReset component
//import "../styles/LoginForm.css";
import "bootstrap/dist/css/bootstrap.min.css";

const LoginForm = ({ onLogin }) => {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [isForgotPassword, setIsForgotPassword] = useState(false); // State to toggle between login and reset

  const handleSubmit = (e) => {
    e.preventDefault();
    setLoading(true);

    // Call backend authentication endpoint
    axios
      .post("https://localhost:7256/api/Authentication/login", { email, password })
      .then((response) => {
        // Log the entire response to check its structure
        console.log("API Response:", response.data); 

        const { token, user } = response.data;
        const { firstName, lastName, roles } = user;

        if (token && roles) {
          // Store authToken and user-specific IDs in localStorage
          localStorage.setItem("jwtToken", token);
          localStorage.setItem("firstName", firstName || "Admin"); // Default to empty string if null
          localStorage.setItem("roles", JSON.stringify(roles));
          localStorage.setItem("customerId", user.customerId); // Store customerId in localStorage
          localStorage.setItem("sellerId", user.sellerId); // Store sellerId in localStorage
          localStorage.setItem("adminId", user.adminId); // Store adminId in localStorage

          // Log the roles array to verify correct data
          console.log("User Roles:", roles);

          // Redirect based on roles
          if (onLogin) {
            onLogin(user);
          }

          // Normalize role comparison (use lowercase to avoid case sensitivity issues)
          if (roles.some(role => role.toLowerCase() === "customer")) {
            window.location.href = "/customer-dashboard"; // Redirect to customer dashboard
          } else if (roles.some(role => role.toLowerCase() === "seller")) {
            window.location.href = "/seller-dashboard"; // Redirect to seller dashboard
          } else if (roles.some(role => role.toLowerCase() === "administrator")) {
            window.location.href = "/admin-dashboard"; // Redirect to admin dashboard
          } else {
            setError("Invalid role. Please contact support.");
          }
        } else {
          setError("Failed to log in. Please try again.");
        }
      })
      .catch((err) => {
        console.error("Login error:", err); // Debugging: Log the error
        setError(err.response?.data?.Message || "Invalid credentials. Please try again.");
      })
      .finally(() => {
        setLoading(false);
      });
  };

  return (
    <div className="auth-form-container d-flex justify-content-center align-items-center" style={{ backgroundColor: "#f7f7f7", height: "100vh", width: "100vw" }}>
      {isForgotPassword ? (
        // Show PasswordReset component if the user clicked 'Forgot Password'
        <PasswordReset />
      ) : (
        // Show Login Form if not in Forgot Password view
        <form className="auth-form p-4 rounded shadow-lg" onSubmit={handleSubmit} style={{ backgroundColor: "#ffffff", width: "100%", maxWidth: "500px" }}>
          <h2 className="text-center mb-4" style={{ fontFamily: "Arial, sans-serif", color: "#007bff" }}>Login</h2>
          {error && <p className="error text-danger">{error}</p>}
          <div className="form-group mb-3">
            <label htmlFor="email" className="font-weight-bold">Email</label>
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
          <div className="form-group mb-3">
            <label htmlFor="password" className="font-weight-bold">Password</label>
            <input
              type="password"
              id="password"
              className="form-control"
              placeholder="Enter your password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
            />
          </div>
          <button type="submit" className="btn btn-primary btn-block w-100" disabled={loading} style={{ backgroundColor: "#ff9900", borderColor: "#ff9900", fontSize: "16px" }}>
            {loading ? "Logging in..." : "Login"}
          </button>
          <p className="text-center mt-3" style={{ fontSize: "14px", color: "#333" }}>
            Don't have an account? <a href="/register" className="text-decoration-none" style={{ color: "#007bff" }}>Sign Up</a>
          </p>
          {/* Forgot Password Link Below Sign Up */}
          <p className="text-center mt-3">
            Forgot your password?{" "}
            <a href="#" onClick={() => setIsForgotPassword(true)} style={{ color: "#007bff" }}>
              Reset Password
            </a>
          </p>
        </form>
      )}
    </div>
  );
};

export default LoginForm;










