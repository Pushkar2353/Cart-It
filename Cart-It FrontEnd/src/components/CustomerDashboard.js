import React, { useState, useEffect } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";
import "bootstrap/dist/css/bootstrap.min.css";

const CustomerDashboard = () => {
  const [customerData, setCustomerData] = useState(null);
  const [orders, setOrders] = useState([]);
  const [payments, setPayments] = useState([]);
  const [reviews, setReviews] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const navigate = useNavigate();

  useEffect(() => {
    const customerId = localStorage.getItem("customerId");
    const jwtToken = localStorage.getItem("jwtToken");

    if (!customerId || !jwtToken) {
      console.warn("No customer ID or JWT token found. Redirecting to login.");
      navigate("/login");
      return;
    }

    // Set up axios default headers for all requests
    axios.defaults.headers.common["Authorization"] = `Bearer ${jwtToken}`;

    // Fetch customer details, orders, payments, and reviews
    axios
      .get(`https://localhost:7256/api/Customer/GetCustomerDetails/${customerId}`)
      .then((response) => {
        console.log("API Response:", response.data);

        // Extract data
        const { customer, orders, payments, reviews } = response.data.data;

        // Set state
        setCustomerData(customer);
        setOrders(orders || []);
        setPayments(payments || []);
        setReviews(reviews || []);
      })
      .catch((error) => {
        console.error("Error fetching customer data:", error);
        setError("Failed to load customer information. Please try again later.");
      })
      .finally(() => {
        setLoading(false);
      });
  }, [navigate]);

  // Loading state
  if (loading) {
    return (
      <div className="d-flex justify-content-center align-items-center vh-100">
        <div className="spinner-border text-primary" role="status">
          <span className="visually-hidden">Loading...</span>
        </div>
      </div>
    );
  }

  // Error state
  if (error) {
    return (
      <div className="d-flex justify-content-center align-items-center vh-100">
        <div className="alert alert-danger text-center" style={{ maxWidth: "400px" }}>
          <h3>Error</h3>
          <p>{error}</p>
          <button className="btn btn-danger" onClick={() => navigate('/login')}>
            Return to Login
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="container mt-4">
      <h2>Welcome, {customerData?.firstName || "Guest"}</h2>
      <div className="bg-light p-3 rounded mb-4">
        <p><strong>Email:</strong> {customerData?.email || "Not available"}</p>
      </div>

      {/* Orders Section */}
      <div className="mb-4">
        <h3>Your Orders</h3>
        {orders.length > 0 ? (
          <div className="row">
            {orders.map((order) => (
              <div key={order.orderId} className="col-md-4 mb-3">
                <div className="card shadow-sm">
                  <div className="card-body">
                    <h5 className="card-title">Order #{order.orderId}</h5>
                    <p className="card-text">
                      <strong>Status:</strong> 
                      <span className={`badge ${order.orderStatus === 'Completed' ? 'bg-success' : order.orderStatus === 'Pending' ? 'bg-warning' : 'bg-secondary'}`}>
                        {order.orderStatus}
                      </span>
                    </p>
                    <p><strong>Product ID:</strong> {order.productId}</p>
                    <p><strong>Quantity:</strong> {order.itemQuantity}</p>
                    <p><strong>Unit Price:</strong> ₹{order.unitPrice || 'N/A'}</p>
                    <p><strong>Total Amount:</strong> ₹{order.totalAmount || 'N/A'}</p>
                    <p><strong>Order Date:</strong> {new Date(order.orderDate).toLocaleDateString()}</p>
                    <p><strong>Shipping Address:</strong> {order.shippingAddress || 'Not provided'}</p>
                  </div>
                </div>
              </div>
            ))}
          </div>
        ) : (
          <p>No orders found.</p>
        )}
      </div>

      {/* Payments Section */}
      <div className="mb-4">
        <h3>Your Payments</h3>
        {payments.length > 0 ? (
          <div className="list-group">
            {payments.map((payment) => (
              <div key={payment.paymentId} className="list-group-item border rounded mb-2">
                <h5 className="mb-1">Payment ID: {payment.paymentId}</h5>
                <p className="mb-1"><strong>Amount:</strong> ₹{payment.amountToPay}</p>
                <p className="mb-1"><strong>Payment Method:</strong> {payment.paymentMethod || 'Not available'}</p>
                <p className="mb-1"><strong>Status:</strong> {payment.paymentStatus || 'Not available'}</p>
                <p className="mb-1"><strong>Payment Date:</strong> {new Date(payment.paymentDate).toLocaleDateString()}</p>
              </div>
            ))}
          </div>
        ) : (
          <p>No payments found.</p>
        )}
      </div>

      {/* Reviews Section */}
      <div className="mb-4">
        <h3>Your Reviews</h3>
        {reviews.length > 0 ? (
          <div className="list-group">
            {reviews.map((review) => (
              <div key={review.reviewId} className="list-group-item border rounded mb-2">
                <h5 className="mb-1">{review.rating} Stars</h5>
                <p className="mb-1"><strong>Product ID:</strong> {review.productId}</p>
                <p className="mb-1"><strong>Review:</strong> {review.reviewText}</p>
                <p className="mb-1"><strong>Date:</strong> {new Date(review.reviewDate).toLocaleDateString()}</p>
              </div>
            ))}
          </div>
        ) : (
          <p>No reviews found.</p>
        )}
      </div>

      {/* Button to go back to Home Page */}
      <button className="btn btn-primary" onClick={() => navigate("/")}>
        Go to Home Page
      </button>
    </div>
  );
};

export default CustomerDashboard;

















