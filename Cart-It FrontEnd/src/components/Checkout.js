import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import "bootstrap/dist/css/bootstrap.min.css";

const Checkout = () => {
  const [productsInCart, setProductsInCart] = useState([]);
  const [totalAmount, setTotalAmount] = useState(0);
  const [message, setMessage] = useState("");
  const [shippingAddress, setShippingAddress] = useState("");
  const navigate = useNavigate();

  // Fetch cart data from localStorage
  useEffect(() => {
    const storedCart = JSON.parse(localStorage.getItem("cart")) || [];
    const storedTotalAmount = parseFloat(localStorage.getItem("totalAmount")) || 0;

    setProductsInCart(storedCart);
    setTotalAmount(storedTotalAmount);

    if (storedCart.length === 0) {
      setMessage("Your cart is empty.");
    }
  }, []);

  // Handle quantity change in the cart
  const handleQuantityChange = (productId, newQuantity) => {
    if (newQuantity < 1) {
      newQuantity = 1;
      setMessage("Quantity must be at least 1.");
    }

    const updatedCart = productsInCart.map((product) =>
      product.productId === productId
        ? {
            ...product,
            cartQuantity: newQuantity,
            amount: product.productPrice * newQuantity,
          }
        : product
    );

    setProductsInCart(updatedCart);
    localStorage.setItem("cart", JSON.stringify(updatedCart));

    // Recalculate the total amount
    const updatedTotalAmount = updatedCart.reduce((sum, product) => sum + product.amount, 0);
    setTotalAmount(updatedTotalAmount);
    localStorage.setItem("totalAmount", updatedTotalAmount);
  };

  const handleConfirmOrder = () => {
    // Prepare the order items data
    const orderItems = productsInCart.map((product) => ({
      productId: product.productId,
      itemQuantity: product.cartQuantity, // Use cartQuantity here to ensure consistency
      unitPrice: parseFloat(product.productPrice),
    }));

    // Ensure the cart is not empty and that the shipping address is provided
    if (productsInCart.length > 0 && shippingAddress.trim()) {
      const customerId = localStorage.getItem("customerId"); // Retrieve customerId from localStorage

      // Prepare the order data based on the provided structure
      const orderData = {
        customerId: parseInt(customerId), // Use the customer ID from localStorage
        orderDate: new Date().toISOString(), // Automatically set the order date
        orderStatus: "inprocess", // Initial order status
        shippingAddress: shippingAddress,
        totalAmount: totalAmount,
        orderItems: orderItems, // Pass the array of items in the cart
      };

      console.log("Order data being saved:", orderData);  // Log order data for debugging

      // Save order data in localStorage (simulating order confirmation)
      localStorage.setItem("orderData", JSON.stringify(orderData));

      // Navigate to payment page with products and totalAmount
      navigate("/payment", { state: { productsInCart, totalAmount } });
    } else {
      setMessage("Please provide a shipping address.");
    }
  };

  return (
    <div className="checkout-page" style={{ minHeight: "100vh" }}>
      <header className="shadow sticky-top" style={{ background: "#1c92d2", color: "#fff" }}>
        <div className="container d-flex justify-content-between align-items-center py-3">
          <h1 className="mb-0" style={{ fontFamily: "Poppins, sans-serif" }}>
            Checkout<span style={{ color: "#fff" }}>-It</span>
          </h1>
        </div>
      </header>

      <div className="container py-5">
        <h2>Checkout</h2>
        {message && <p className="message">{message}</p>}
        {productsInCart.length === 0 ? (
          <p>Your cart is empty! Add some products before proceeding.</p>
        ) : (
          productsInCart.map((product) => (
            <div key={product.productId} className="checkout-item shadow-sm p-3 mb-3 rounded">
              <div className="d-flex justify-content-between align-items-center">
                <img
                  src={product.productImageUrl}
                  alt={product.productName}
                  style={{ width: "50px", height: "50px", objectFit: "cover" }}
                />
                <div className="product-details" style={{ marginLeft: "10px" }}>
                  <h5>{product.productName}</h5>
                  <p>{product.productDescription}</p>
                </div>
                <p className="ms-auto">₹{parseFloat(product.productPrice).toFixed(2)}</p>
              </div>
              <div className="d-flex align-items-center">
                <span className="mx-2">Quantity: </span>
                <input
                  type="number"
                  value={product.cartQuantity}
                  min="1"
                  onChange={(e) => handleQuantityChange(product.productId, parseInt(e.target.value))}
                  className="form-control"
                  style={{ width: "80px" }}
                />
              </div>
            </div>
          ))
        )}
        {productsInCart.length > 0 && (
          <>
            <h4 className="mt-3">Total Amount: ₹{isNaN(totalAmount) ? 0 : totalAmount.toFixed(2)}</h4>
            <div className="form-group mt-4">
              <label htmlFor="shippingAddress">Shipping Address:</label>
              <textarea
                id="shippingAddress"
                value={shippingAddress}
                onChange={(e) => setShippingAddress(e.target.value)}
                className="form-control"
                rows="4"
                placeholder="Enter your shipping address"
              ></textarea>
            </div>
            <button className="btn btn-primary mt-4" onClick={handleConfirmOrder}>
              Confirm Order
            </button>
          </>
        )}
      </div>

      <footer className="text-white text-center py-3 mt-5" style={{ background: "#141e30" }}>
        <p>© 2024 Cart-It. All Rights Reserved.</p>
      </footer>
    </div>
  );
};

export default Checkout;





























