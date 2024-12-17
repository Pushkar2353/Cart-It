import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import "bootstrap/dist/css/bootstrap.min.css";
import { FaCheckCircle } from "react-icons/fa"; // You can use react-icons for the green tick icon

const OrderConfirmation = () => {
  const navigate = useNavigate();
  const [isPaymentSuccessful, setIsPaymentSuccessful] = useState(true); // Set to true assuming payment was successful
  const [showTick, setShowTick] = useState(false);

  useEffect(() => {
    // Show the green tick icon for 5 seconds
    if (isPaymentSuccessful) {
      setShowTick(true);
      setTimeout(() => {
        setShowTick(false);
        // After 5 seconds, navigate to the homepage or another page
        navigate("/");
      }, 5000); // 5 seconds
    }
  }, [isPaymentSuccessful, navigate]);

  return (
    <div className="order-confirmation-page" style={{ minHeight: "100vh" }}>
      <header className="shadow sticky-top" style={{ background: "#1c92d2", color: "#fff" }}>
        <div className="container d-flex justify-content-between align-items-center py-3">
          <h1 className="mb-0" style={{ fontFamily: "Poppins, sans-serif" }}>
            Order<span style={{ color: "#fff" }}>-Confirmation</span>
          </h1>
        </div>
      </header>

      <div className="container py-5 text-center">
        <h2>Payment Successful</h2>
        <p>Your payment has been successfully processed.</p>

        {showTick && (
          <div className="my-5">
            <FaCheckCircle size={100} color="green" />
            <p className="text-success mt-3" style={{ fontSize: "24px", fontWeight: "bold" }}>
              Payment Confirmed!
            </p>
          </div>
        )}

        <p className="mt-4">You will be redirected to the homepage shortly.</p>
      </div>

      <footer className="text-white text-center py-3 mt-5" style={{ background: "#141e30" }}>
        <p>© 2024 Cart-It. All Rights Reserved.</p>
      </footer>
    </div>
  );
};

export default OrderConfirmation;
