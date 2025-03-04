document.addEventListener("DOMContentLoaded", function () {
    const sidebarMenuItems = document.querySelectorAll(".sidebar-menu li");

    // Add click event to each menu item
    sidebarMenuItems.forEach(item => {
        item.addEventListener("click", function () {
            // Remove active class from all items
            sidebarMenuItems.forEach(li => li.classList.remove("active"));
            // Add active class to the clicked item
            this.classList.add("active");
        });
    });

    //// Dropdown functionality
    //const userDropdown = document.querySelector(".user-dropdown");
    //const userDropdownContent = document.querySelector(".user-dropdown-content");

    //userDropdown.addEventListener("click", function (e) {
    //    e.preventDefault();
    //    userDropdownContent.style.display = userDropdownContent.style.display === "block" ? "none" : "block";
    //});

    //// Close dropdown when clicking outside
    //document.addEventListener("click", function (e) {
    //    if (!userDropdown.contains(e.target)) {
    //        userDropdownContent.style.display = "none";
    //    }
    //});
});