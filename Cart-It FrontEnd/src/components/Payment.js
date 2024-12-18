import React, { useState, useEffect } from "react";
import axios from "axios";
import { useLocation, useNavigate } from "react-router-dom";
import "bootstrap/dist/css/bootstrap.min.css";

const Payment = () => {
  const location = useLocation();
  const navigate = useNavigate();
  
  // Retrieving data from localStorage
  const customerId = localStorage.getItem("customerId");
  const orderData = JSON.parse(localStorage.getItem("orderData")) || {};
  const [paymentData, setPaymentData] = useState({
    paymentId: 0,
    orderId: 5, // Update this based on the actual order ID
    customerId: customerId,
    amountToPay: orderData?.totalAmount || 0,
    paymentMethod: "",
    paymentStatus: "Pending",
    paymentDate: new Date().toISOString(),
    productImage: orderData?.productImage || "",
  });
  const [paymentStatus, setPaymentStatus] = useState(null);
  const [errorMessage, setErrorMessage] = useState("");

  useEffect(() => {
    if (!orderData || !orderData?.totalAmount) {
      setErrorMessage("Order data is not available.");
    }
  }, [orderData]);

  const handlePayment = async () => {
    if (paymentData) {
      try {
        // Sending the payment data to the backend for processing
        const response = await axios.post("https://localhost:7256/api/Payment", paymentData, {
          headers: {
            "Content-Type": "application/json",
          },
        });

        if (response.status === 201) {
          setPaymentStatus("Payment successful.");
          // Navigate to order confirmation page
          setTimeout(() => navigate("/orderConfirmation"), 2000);
        } else {
          setPaymentStatus("Payment failed. Please try again.");
        }
      } catch (error) {
        console.error("Payment API Error:", error);
        setPaymentStatus("An error occurred while processing the payment.");
      }
    }
  };

  const handlePayNow = () => {
    // Automatically mark payment as successful
    setPaymentData((prevData) => ({
      ...prevData,
      paymentStatus: "Success",
    }));
    setPaymentStatus("Payment successful.");
    handlePayment(); // Trigger payment processing
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setPaymentData((prevData) => ({
      ...prevData,
      [name]: value,
    }));
  };

  return (
    <div className="payment-page" style={{ minHeight: "100vh" }}>
      <header className="shadow sticky-top" style={{ background: "#1c92d2", color: "#fff" }}>
        <div className="container d-flex justify-content-between align-items-center py-3">
          <h1 className="mb-0" style={{ fontFamily: "Poppins, sans-serif" }}>
            Payment<span style={{ color: "#fff" }}>-Page</span>
          </h1>
        </div>
      </header>

      <div className="container py-5">
        <h2>Payment Details</h2>

        {errorMessage && <p className="text-danger">{errorMessage}</p>}

        <div className="payment-details">
          {paymentData.productImage && (
            <div className="product-image">
              <img src={paymentData.productImage} alt="Product" style={{ maxWidth: "200px" }} />
            </div>
          )}

          <p>Amount to Pay: ₹{paymentData.amountToPay}</p>

          <div className="form-group">
            <label htmlFor="paymentMethod">Payment Method</label>
            <select
              id="paymentMethod"
              name="paymentMethod"
              className="form-control"
              value={paymentData.paymentMethod}
              onChange={handleChange}
            >
              <option value="">Select Payment Method</option>
              <option value="CreditCard">Credit Card</option>
              <option value="DebitCard">Debit Card</option>
              <option value="UPI">UPI</option>
              <option value="CashOnDelivery">Cash on Delivery</option>
            </select>
          </div>

          {/* Pay Now Button */}
          <button
            className="btn btn-success mt-4"
            onClick={handlePayNow}
            disabled={!paymentData.paymentMethod} // Disable if no payment method selected
          >
            Pay Now
          </button>

          <p className="mt-3">Payment Date: {new Date(paymentData.paymentDate).toLocaleString()}</p>
        </div>

        {paymentStatus && <p className="mt-4">{paymentStatus}</p>}
      </div>

      <footer className="text-white text-center py-3 mt-5" style={{ background: "#141e30" }}>
        <p>© 2024 Cart-It. All Rights Reserved.</p>
      </footer>
    </div>
  );
};

export default Payment;



