import React, { useState, useEffect } from "react";
import axios from "axios";
import { useLocation, useNavigate } from "react-router-dom";
import "bootstrap/dist/css/bootstrap.min.css";

const Payment = () => {
  const location = useLocation();
  const navigate = useNavigate();
  
  // Retrieving data from localStorage
  const customerId = localStorage.getItem("customerId");
  const orderData = JSON.parse(localStorage.getItem("orderData")) || {}; // Retrieve the order data stored previously
  const [paymentData, setPaymentData] = useState({
    paymentId: 0,
    orderId: 5, // You can update this based on the actual order ID
    customerId: customerId,
    amountToPay: orderData?.totalAmount || 0,
    paymentMethod: "",
    paymentStatus: "Pending", // Default payment status
    paymentDate: new Date().toISOString(),
    productImage: orderData?.productImage || "", // Assuming product image is stored in order data
  });
  const [paymentStatus, setPaymentStatus] = useState(null);
  const [errorMessage, setErrorMessage] = useState("");

  useEffect(() => {
    // Retrieve payment data passed from the Checkout page
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
          // Optionally navigate to the order confirmation page or another page
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
          {/* Product Image */}
          {paymentData.productImage && (
            <div className="product-image">
              <img src={paymentData.productImage} alt="Product" style={{ maxWidth: "200px" }} />
            </div>
          )}

          {/* Amount to Pay */}
          <p>Amount to Pay: ₹{paymentData.amountToPay}</p>

          {/* Payment Method Dropdown */}
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

          {/* Payment Status Dropdown */}
          <div className="form-group">
            <label htmlFor="paymentStatus">Payment Status</label>
            <select
              id="paymentStatus"
              name="paymentStatus"
              className="form-control"
              value={paymentData.paymentStatus}
              onChange={handleChange}
            >
              <option value="Pending">Pending</option>
              <option value="Success">Success</option>
              <option value="Failed">Failed</option>
            </select>
          </div>

          {/* Payment Date (auto-generated) */}
          <p>Payment Date: {new Date(paymentData.paymentDate).toLocaleString()}</p>

          {/* Button to initiate the payment */}
          <button className="btn btn-primary mt-4" onClick={handlePayment}>
            Proceed with Payment
          </button>
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


