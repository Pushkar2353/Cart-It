import React from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import HomePage from "./components/HomePage";
import Register from "./components/Register";
import LoginForm from "./components/LoginForm";
import Cart from "./components/CartPage";
import CustomerDashboard from './components/CustomerDashboard';
import SellerDashboard from './components/SellerDashboard';
import AdminDashboard from './components/AdminDashboard';
import Products from "./components/Products";
import Category from "./components/Category";
import Checkout from "./components/Checkout";
import Payment from "./components/Payment";
import OrderConfirmation from "./components/OrderConformation";
import Review from "./components/Review";
import EditProfile from "./components/EditProfile";

function App() {
  return (
    <Router>
      <div>
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/register" element={<Register />} />
          <Route path="/login" element={<LoginForm />} />
          <Route path="/customer-dashboard" element={<CustomerDashboard />} />
          <Route path="/seller-dashboard" element={<SellerDashboard />} />
          <Route path="/admin-dashboard" element={<AdminDashboard />} />
          <Route path="/cart" element={<Cart />} />
          <Route path="/products" element={<Products/>} />
          <Route path="/category" element={<Category/>} />
          <Route path="/checkout" element={<Checkout/>} />
          <Route path="/payment" element={<Payment/>} />
          <Route path="/orderConfirmation" element={<OrderConfirmation/>} />
          <Route path="/review" element={<Review/>} />
          <Route path="/edit-profile" element={<EditProfile />} />
        </Routes>
      </div>
    </Router>
  );
}

export default App;


