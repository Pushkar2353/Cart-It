import React, { useState, useEffect } from "react";
import axios from "axios";
import { ShoppingCart } from "lucide-react";
import { useNavigate } from "react-router-dom"; // Import useNavigate from React Router
import "bootstrap/dist/css/bootstrap.min.css";

const CartPage = () => {
  const [productsInCart, setProductsInCart] = useState([]);
  const [totalAmount, setTotalAmount] = useState(0);
  const [message, setMessage] = useState("");
  const [cartCount, setCartCount] = useState(0);
  const [customerId, setCustomerId] = useState(null);
  const [cartUpdated, setCartUpdated] = useState(false);

  const navigate = useNavigate(); // Hook to programmatically navigate

  // Fetch customerId from localStorage and load cart data
  useEffect(() => {
    const storedCustomerId = localStorage.getItem("customerId");
    if (storedCustomerId) {
      setCustomerId(storedCustomerId);
      const storedCart = JSON.parse(localStorage.getItem("cart")) || [];
      if (storedCart.length > 0) {
        setProductsInCart(storedCart);
        setCartCount(storedCart.length);
        const total = storedCart.reduce((sum, product) => sum + product.amount, 0);
        setTotalAmount(total);
        localStorage.setItem("totalAmount", total.toFixed(2)); // Store the total amount
      } else {
        setMessage("Your cart is empty.");
      }
    } else {
      setMessage("No customer ID found. Please log in.");
    }
  }, [cartUpdated]);

  // Handle adding a product to the cart
  const handleAddToCart = async (product) => {
    try {
      const cartData = {
        customerId: customerId,
        productId: product.productId,
        productName: product.productName,
        productPrice: product.productPrice,
        productDescription: product.productDescription,
        productImageUrl: product.productImageUrl,
        cartQuantity: 1,
        amount: product.productPrice,
      };

      const response = await axios.post("https://localhost:7256/api/Cart", cartData);
      if (response.status === 201) {
        const createdCart = response.data;
        setProductsInCart((prevCart) => [...prevCart, createdCart]);
        setCartCount((prevCount) => prevCount + 1);
        setTotalAmount((prevTotal) => prevTotal + createdCart.amount);
        setMessage("Product added to cart successfully.");
        // Store updated cart in localStorage
        localStorage.setItem("cart", JSON.stringify([...productsInCart, createdCart]));
      } else {
        setMessage("Failed to add product to the cart.");
      }
    } catch (error) {
      console.error("Error adding product to cart:", error);
      setMessage("An error occurred while adding the product to the cart.");
    }
  };

  // Handle updating quantity in the cart
  const handleUpdateQuantity = async (productId, newQuantity) => {
    try {
      if (newQuantity <= 0) return;

      const updatedProduct = productsInCart.find((product) => product.productId === productId);
      if (!updatedProduct) {
        console.error("Product not found in the cart");
        return;
      }

      const oldAmount = updatedProduct.amount;
      updatedProduct.cartQuantity = newQuantity;
      updatedProduct.amount = updatedProduct.productPrice * newQuantity;
      const updatedCart = productsInCart.map((product) =>
        product.productId === productId ? { ...product, cartQuantity: newQuantity } : product
      );

      const response = await axios.put(`https://localhost:7256/api/Cart/${productId}`, updatedProduct);
      if (response.status === 200) {
        setProductsInCart((prevCart) =>
          prevCart.map((product) =>
            product.productId === productId ? updatedProduct : product
          )
        );
        setTotalAmount((prevTotal) => prevTotal - oldAmount + updatedProduct.amount);
        setMessage("Quantity updated successfully.");
        // Update localStorage with new cart data
        localStorage.setItem("cart", JSON.stringify(productsInCart));
        localStorage.setItem("cart", JSON.stringify(updatedCart));
      } else {
        setMessage("Error updating cart product.");
      }
    } catch (error) {
      console.error("Error updating cart product:", error);
      setMessage("Error updating cart product.");
    }
  };

  // Handle removing product from the cart
  const handleRemoveFromCart = async (productId) => {
    try {
      await axios.delete(`https://localhost:7256/api/Cart/${productId}`);
      setProductsInCart((prevCart) => prevCart.filter((product) => product.productId !== productId));
      const removedProduct = productsInCart.find((product) => product.productId === productId);
      setTotalAmount((prevTotal) => prevTotal - removedProduct.amount);
      setCartCount((prevCount) => prevCount - 1);
      setMessage("Product removed from cart.");
      // Update localStorage by removing the item
      const updatedCart = productsInCart.filter((product) => product.productId !== productId);
      localStorage.setItem("cart", JSON.stringify(updatedCart));
    } catch (error) {
      setMessage("Error removing product from cart.");
      console.error("Error removing product from cart:", error);
    }
  };

  // Navigate to the checkout page
  const handleProceedToCheckout = () => {
    if (productsInCart.length === 0) {
      setMessage("Please add products to your cart before proceeding to checkout.");
    } else {
      navigate("/checkout"); // Navigate to the checkout page
    }
  };

  return (
    <div className="cart-page" style={{ minHeight: "100vh" }}>
      <header className="shadow sticky-top" style={{ background: "#1c92d2", color: "#fff" }}>
        <div className="container d-flex justify-content-between align-items-center py-3">
          <div className="d-flex align-items-center">
            <h1 className="mb-0" style={{ fontFamily: "Poppins, sans-serif" }}>
              Cart<span style={{ color: "#fff" }}>-It</span>
            </h1>
          </div>
          <nav className="d-flex align-items-center">
            <a href="/cart" className="text-dark position-relative">
              <ShoppingCart size={24} />
              <span
                className="badge bg-primary rounded-pill position-absolute top-0 start-100 translate-middle"
                style={{ fontSize: "0.75rem" }}
              >
                {cartCount}
              </span>
            </a>
          </nav>
        </div>
      </header>

      <div className="container py-5">
        <h2>Your Cart</h2>
        {message && <p className="message">{message}</p>}
        {productsInCart.length === 0 ? (
          <p>Your cart is empty!</p>
        ) : (
          productsInCart.map((product) => (
            <div key={product.productId} className="cart-item shadow-sm p-3 mb-3 rounded">
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
                <p className="ms-auto">₹{product.productPrice}</p>
              </div>
              <div className="d-flex align-items-center">
                <button
                  onClick={() => handleUpdateQuantity(product.productId, product.cartQuantity - 1)}
                  className="btn btn-sm btn-outline-primary"
                >
                  -
                </button>
                <span className="mx-2">{product.cartQuantity}</span>
                <button
                  onClick={() => handleUpdateQuantity(product.productId, product.cartQuantity + 1)}
                  className="btn btn-sm btn-outline-primary"
                >
                  +
                </button>
              </div>
              <button
                onClick={() => handleRemoveFromCart(product.productId)}
                className="btn btn-sm btn-danger mt-2"
              >
                Remove
              </button>
            </div>
          ))
        )}
        {productsInCart.length > 0 && (
          <h4>Total Amount: ₹{totalAmount.toFixed(2)}</h4>
        )}
        <div className="d-flex justify-content-between mt-4">
          <button className="btn btn-primary" onClick={handleProceedToCheckout}>
            Proceed to Checkout
          </button>
        </div>
      </div>

      <footer className="text-white text-center py-3 mt-5" style={{ background: "#141e30" }}>
        <p>© 2024 Cart-It. All Rights Reserved.</p>
      </footer>
    </div>
  );
};

export default CartPage;

























