import React, { useState } from 'react';
import Slider from 'react-slick'; // Assuming you are using this library for the slider

const ProductSection = ({ filteredProducts }) => {
  // State to manage the cart
  const [cartItems, setCartItems] = useState([]);

  // Handle adding a product to the cart
  const handleAddToCart = (product) => {
    // Add the product to the cart
    setCartItems([...cartItems, product]);
    console.log('Cart Items:', cartItems); // To verify that the product is added
  };

  return (
    <main className="container">
      <h2 className="text-center my-4 text-primary">Top Picks</h2>
      <Slider {...settings}>
        {filteredProducts.map((product) => (
          <div
            key={product.productId}
            className="card mx-2 shadow-sm"
            style={{
              borderRadius: "8px",
              transition: "transform 0.3s",
            }}
          >
            <img
              src={product.productImageUrl}
              className="card-img-top"
              alt={product.productName}
              style={{ height: "200px", objectFit: "cover" }}
            />
            <div className="card-body text-center">
              <h5 className="card-title">{product.productName}</h5>
              <p className="card-text">₹{product.productPrice}</p>
              <button
                className="btn btn-sm btn-primary"
                onClick={() => handleAddToCart(product)} // Adding the product to the cart when clicked
              >
                Add to Cart
              </button>
            </div>
          </div>
        ))}
      </Slider>
    </main>
  );
};

export default ProductSection;
