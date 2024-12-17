import React, { useState, useEffect } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";
import "bootstrap/dist/css/bootstrap.min.css";

const AdminDashboard = () => {
  const [customers, setCustomers] = useState([]);
  const [sellers, setSellers] = useState([]);
  const [products, setProducts] = useState([]);
  const [categories, setCategories] = useState([]);
  const [orders, setOrders] = useState([]);
  const [payments, setPayments] = useState([]);
  const [reviews, setReviews] = useState([]);
  const [selectedSection, setSelectedSection] = useState("Customers"); // For dropdown selection
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const navigate = useNavigate();

  useEffect(() => {
    const adminId = localStorage.getItem("adminId");
    const jwtToken = localStorage.getItem("jwtToken");

    if (!adminId || !jwtToken) {
      console.warn("No admin ID or JWT token found. Redirecting to login.");
      navigate("/login");
      return;
    }

    // Set up axios default headers for all requests
    axios.defaults.headers.common["Authorization"] = `Bearer ${jwtToken}`;

    // Fetch data for all sections
    const fetchData = async () => {
      try {
        const [
          customersResponse,
          sellersResponse,
          productsResponse,
          categoriesResponse,
          ordersResponse,
          paymentsResponse,
          reviewsResponse,
        ] = await Promise.all([
          axios.get("https://localhost:7256/api/administrator/customers"),
          axios.get("https://localhost:7256/api/administrator/sellers"),
          axios.get("https://localhost:7256/api/administrator/products"),
          axios.get("https://localhost:7256/api/administrator/categories"),
          axios.get("https://localhost:7256/api/administrator/orders"),
          axios.get("https://localhost:7256/api/administrator/payments"),
          axios.get("https://localhost:7256/api/administrator/reviews"),
        ]);

        setCustomers(customersResponse.data || []);
        setSellers(sellersResponse.data || []);
        setProducts(productsResponse.data || []);
        setCategories(categoriesResponse.data || []);
        setOrders(ordersResponse.data || []);
        setPayments(paymentsResponse.data || []);
        setReviews(reviewsResponse.data || []);
      } catch (error) {
        console.error("Error fetching data:", error);
        setError("Error fetching data. Please try again later.");
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [navigate]);

  // Loading state
  if (loading) {
    return (
      <div className="loading-container text-center my-5">
        <div className="spinner-border text-primary" role="status">
          <span className="sr-only">Loading...</span>
        </div>
      </div>
    );
  }

  // Error state
  if (error) {
    return (
      <div className="error-container text-center my-5">
        <div className="alert alert-danger">
          <h3>Error</h3>
          <p>{error}</p>
          <button className="btn btn-primary" onClick={() => navigate("/login")}>
            Return to Login
          </button>
        </div>
      </div>
    );
  }

  // Render data based on the selected section
  const renderSectionData = () => {
    switch (selectedSection) {
      case "Customers":
        return (
          <div>
            <h3>Customers</h3>
            {customers.map((customer) => (
              <div key={customer.customerId} className="card mb-3">
                <div className="card-body">
                  <p><strong>Name:</strong> {customer.firstName} {customer.lastName}</p>
                  <p><strong>Email:</strong> {customer.email}</p>
                  <p><strong>Phone:</strong> {customer.phoneNumber}</p>
                  <p><strong>Address:</strong> {customer.addressLine1}, {customer.city}, {customer.state}, {customer.pinCode}</p>
                </div>
              </div>
            ))}
          </div>
        );

      case "Sellers":
        return (
          <div>
            <h3>Sellers</h3>
            {sellers.map((seller) => (
              <div key={seller.sellerId} className="card mb-3">
                <div className="card-body">
                  <p><strong>Company:</strong> {seller.companyName}</p>
                  <p><strong>Name:</strong> {seller.firstName} {seller.lastName}</p>
                  <p><strong>GSTIN:</strong> {seller.gstin}</p>
                  <p><strong>Address:</strong> {seller.addressLine1}, {seller.city}, {seller.state}, {seller.pinCode}</p>
                </div>
              </div>
            ))}
          </div>
        );

      case "Products":
        return (
          <div>
            <h3>Products</h3>
            {products.map((product) => (
              <div key={product.productId} className="card mb-3">
                <div className="card-body">
                  <p><strong>Name:</strong> {product.productName}</p>
                  <p><strong>Price:</strong> ₹{product.productPrice}</p>
                  <p><strong>Description:</strong> {product.productDescription}</p>
                  <p><strong>Stock:</strong> {product.productStock}</p>
                  <img src={product.productImageUrl} alt={product.productName} className="img-fluid" />
                </div>
              </div>
            ))}
          </div>
        );

      case "Categories":
        return (
          <div>
            <h3>Categories</h3>
            {categories.map((category) => (
              <div key={category.categoryId} className="card mb-3">
                <div className="card-body">
                  <p><strong>Category Name:</strong> {category.categoryName}</p>
                </div>
              </div>
            ))}
          </div>
        );

      case "Orders":
        return (
          <div>
            <h3>Orders</h3>
            {orders.map((order) => (
              <div key={order.orderId} className="card mb-3">
                <div className="card-body">
                  <p><strong>Order ID:</strong> {order.orderId}</p>
                  <p><strong>Customer ID:</strong> {order.customerId}</p>
                  <p><strong>Status:</strong> {order.orderStatus}</p>
                  <p><strong>Total Amount:</strong> ₹{order.totalAmount}</p>
                  <p><strong>Shipping Address:</strong> {order.shippingAddress}</p>
                </div>
              </div>
            ))}
          </div>
        );

      case "Payments":
        return (
          <div>
            <h3>Payments</h3>
            {payments.map((payment) => (
              <div key={payment.paymentId} className="card mb-3">
                <div className="card-body">
                  <p><strong>Payment ID:</strong> {payment.paymentId}</p>
                  <p><strong>Order ID:</strong> {payment.orderId}</p>
                  <p><strong>Amount:</strong> ₹{payment.amountToPay}</p>
                  <p><strong>Status:</strong> {payment.paymentStatus}</p>
                </div>
              </div>
            ))}
          </div>
        );

      case "Reviews":
        return (
          <div>
            <h3>Reviews</h3>
            {reviews.map((review) => (
              <div key={review.reviewId} className="card mb-3">
                <div className="card-body">
                  <p><strong>Customer ID:</strong> {review.customerId}</p>
                  <p><strong>Product ID:</strong> {review.productId}</p>
                  <p><strong>Rating:</strong> {review.rating}</p>
                  <p><strong>Review:</strong> {review.reviewText}</p>
                </div>
              </div>
            ))}
          </div>
        );

      default:
        return <p>Select a section to view data.</p>;
    }
  };

  return (
    <div className="container">
      <h1 className="text-center my-4">Admin Dashboard</h1>
      <div className="form-group">
        <label htmlFor="dataSectionDropdown">Select Data Section:</label>
        <select
          id="dataSectionDropdown"
          className="form-control"
          value={selectedSection}
          onChange={(e) => setSelectedSection(e.target.value)}
        >
          <option>Customers</option>
          <option>Sellers</option>
          <option>Products</option>
          <option>Categories</option>
          <option>Orders</option>
          <option>Payments</option>
          <option>Reviews</option>
        </select>
      </div>

      <div className="section-data mt-4">{renderSectionData()}</div>

{/* Buttons with Flexbox */}
<div className="d-flex justify-content-start gap-2 mt-4">
  <button className="btn btn-primary" onClick={() => navigate("/")}>
    Go to Home Page
  </button>
  <button className="btn btn-primary" onClick={() => navigate("/category")}>
    Go To Category Page
  </button>
</div>
</div>
  );
};

export default AdminDashboard;


