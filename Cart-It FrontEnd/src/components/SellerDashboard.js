import React, { useState, useEffect } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";
import "bootstrap/dist/css/bootstrap.min.css";

const SellerDashboard = () => {
  const [sellerData, setSellerData] = useState(null);
  const [orders, setOrders] = useState([]);
  const [reviews, setReviews] = useState([]);
  const [payments, setPayments] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const navigate = useNavigate();

  useEffect(() => {
    const sellerId = localStorage.getItem("sellerId");
    const jwtToken = localStorage.getItem("jwtToken");

    if (!sellerId || !jwtToken) {
      console.warn("No seller ID or JWT token found. Redirecting to login.");
      navigate("/login");
      return;
    }

    // Set up axios default headers for all requests
    axios.defaults.headers.common["Authorization"] = `Bearer ${jwtToken}`;

    // Fetch seller details, orders, reviews, and payments
    axios
      .get(`https://localhost:7256/api/Seller/GetSellerDashboard/${sellerId}`)
      .then((response) => {
        console.log("API Response:", response.data);

        // Extract data
        const { seller, orders, reviews, payments } = response.data.data;

        // Set state
        setSellerData(seller);
        setOrders(orders || []);
        setReviews(reviews || []);
        setPayments(payments || []);
      })
      .catch((error) => {
        console.error("Error fetching seller data:", error);
        setError("Failed to load seller information. Please try again later.");
      })
      .finally(() => {
        setLoading(false);
      });
  }, [navigate]);

  // Loading state
  if (loading) {
    return (
      <div className="loading-container">
        <div className="spinner">Loading...</div>
      </div>
    );
  }

  // Error state
  if (error) {
    return (
      <div className="error-container">
        <div className="error-message">
          <h3>Error</h3>
          <p>{error}</p>
          <button onClick={() => navigate("/login")}>Return to Login</button>
        </div>
      </div>
    );
  }

  return (
    <div className="seller-dashboard container d-flex flex-column" style={{ minHeight: '100vh' }}>
      <h2>Welcome, {sellerData?.firstName} {sellerData?.lastName}</h2>
      <div className="seller-info">
        <p><strong>Email:</strong> {sellerData?.email}</p>
        <p><strong>Store Name:</strong> {sellerData?.companyName}</p>
        <p><strong>Phone Number:</strong> {sellerData?.sellerPhoneNumber}</p>
        <p><strong>Address:</strong> {sellerData?.addressLine1}, {sellerData?.city}, {sellerData?.state}</p>
      </div>

      {/* Orders Section */}
      <div className="orders-section">
        <h3>Your Orders</h3>
        {orders.length > 0 ? (
          <div className="orders-list grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            {orders.map((order) => (
              <div key={order.orderId} className="order-card border rounded-lg p-4 mb-4">
                <div className="order-header flex justify-between items-center mb-2">
                  <span className="order-id font-bold text-lg">Order #{order.orderId}</span>
                  <span className={`order-status px-2 py-1 rounded text-sm ${order.orderStatus === 'Completed' ? 'bg-green-200 text-green-800' : order.orderStatus === 'Pending' ? 'bg-yellow-200 text-yellow-800' : 'bg-gray-200 text-gray-800'}`}>
                    {order.orderStatus}
                  </span>
                </div>
                <div className="order-info">
                  <p><strong>Product ID:</strong> {order.productId}</p>
                  <p><strong>Quantity:</strong> {order.itemQuantity}</p>
                  <p><strong>Unit Price:</strong> ₹{order.unitPrice || 'N/A'}</p>
                  <p><strong>Total Amount:</strong> ₹{order.totalAmount || 'N/A'}</p>
                  <p><strong>Order Date:</strong> {new Date(order.orderDate).toLocaleDateString()}</p>
                  <p><strong>Shipping Address:</strong> {order.shippingAddress || 'Not provided'}</p>
                </div>
              </div>
            ))}
          </div>
        ) : (
          <p>No orders found.</p>
        )}
      </div>

      {/* Reviews Section */}
      <div className="reviews-section">
        <h3>Your Product Reviews</h3>
        {reviews.length > 0 ? (
          <div className="reviews-list">
            {reviews.map((review) => (
              <div key={review.reviewId} className="review-card border rounded-lg p-4 mb-4">
                <div className="review-header flex justify-between items-center mb-2">
                  <span className="review-rating">{review.rating} stars</span>
                  <span className="review-date">{new Date(review.reviewDate).toLocaleDateString()}</span>
                </div>
                <div className="review-content">
                  <p><strong>Product ID:</strong> {review.productId}</p>
                  <p><strong>Review:</strong> {review.reviewText}</p>
                </div>
              </div>
            ))}
          </div>
        ) : (
          <p>No reviews found.</p>
        )}
      </div>

      {/* Payments Section */}
      <div className="payments-section">
        <h3>Your Payments</h3>
        {payments.length > 0 ? (
          <div className="payments-list">
            {payments.map((payment) => (
              <div key={payment.paymentId} className="payment-card border rounded-lg p-4 mb-4">
                <div className="payment-details">
                  <p><strong>Payment ID:</strong> {payment.paymentId}</p>
                  <p><strong>Amount:</strong> ₹{payment.amountToPay}</p>
                  <p><strong>Payment Method:</strong> {payment.paymentMethod || 'Not available'}</p>
                  <p><strong>Payment Status:</strong> {payment.paymentStatus || 'Not available'}</p>
                  <p><strong>Payment Date:</strong> {new Date(payment.paymentDate).toLocaleDateString()}</p>
                </div>
              </div>
            ))}
          </div>
        ) : (
          <p>No payments found.</p>
        )}
      </div>

      {/* Button to go back to Home Page */}
      <div className="mt-auto">
        <button className="btn btn-primary" onClick={() => navigate("/")}>
          Go to Home Page
        </button>
      </div>
      <div className="mt-auto">
        <button className="btn btn-primary" onClick={() => navigate("/products")}>
          Go To Products Page
        </button>
      </div>
    </div>
  );
};

export default SellerDashboard;






