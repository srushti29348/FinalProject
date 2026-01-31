import { useState, useRef, useEffect } from "react";
import { NavLink, useNavigate } from "react-router-dom";
import "./Navbar.css";
import logo from "../logos/Horizontal_logo-removebg-preview-white (1).png";

function Navbar() {
  const nav = useNavigate();

  const user = JSON.parse(sessionStorage.getItem("user"));

  const firstLetter =
    user?.fullName && typeof user.fullName === "string"
      ? user.fullName.charAt(0).toUpperCase()
      : "";

  const [open, setOpen] = useState(false);
  const menuRef = useRef();

  // Close dropdown on outside click
  useEffect(() => {
    const handler = (e) => {
      if (menuRef.current && !menuRef.current.contains(e.target)) {
        setOpen(false);
      }
    };
    document.addEventListener("mousedown", handler);
    return () => document.removeEventListener("mousedown", handler);
  }, []);

  // Close dropdown on login/logout change
  useEffect(() => {
    setOpen(false);
  }, [user?.userId]);

  const logout = () => {
    sessionStorage.clear();
    nav("/login");
  };

  return (
    <div className="navbar-wrapper">
      <div className="navbar-inner">

        {/* LEFT → LOGO */}
        <div className="navbar-title" onClick={() => nav("/")}>
          <img src={logo} alt="DACPro" className="navbar-logo" />
        </div>

        {/* CENTER */}
        <div className="navbar-tabs">
          <NavLink to="/" className={({ isActive }) => isActive ? "tab active" : "tab"}>
            Home
          </NavLink>

          <NavLink to="/about" className={({ isActive }) => isActive ? "tab active" : "tab"}>
            About Us
          </NavLink>

          <NavLink to="/contact" className={({ isActive }) => isActive ? "tab active" : "tab"}>
            Contact Us
          </NavLink>

          {user && (
            <NavLink
              to="/dashboard"
              className={({ isActive }) => isActive ? "tab active" : "tab"}
            >
              Dashboard
            </NavLink>
          )}
        </div>

        {/* RIGHT */}
        {!user ? (
          <div className="navbar-tabs">
            <NavLink to="/login" className={({ isActive }) => isActive ? "tab active" : "tab"}>
              Sign In
            </NavLink>

            <NavLink to="/register" className={({ isActive }) => isActive ? "tab active" : "tab"}>
              Sign Up
            </NavLink>
          </div>
        ) : (
          <div className="profile-area" ref={menuRef}>
            <div className="profile-chip" onClick={() => setOpen(!open)}>
              <span>Welcome, {user?.fullName || "User"}</span>

              {/* 🔐 SAFE AVATAR */}
              {firstLetter && (
                <div className="avatar">
                  {firstLetter}
                </div>
              )}
            </div>

            {open && (
              <div className="profile-dropdown">
                <p className="profile-name">
                  Hi, {user?.fullName || "User"}!
                </p>
                <button className="logout-btn" onClick={logout}>
                  Log out
                </button>
              </div>
            )}
          </div>
        )}

      </div>
    </div>
  );
}

export default Navbar;
