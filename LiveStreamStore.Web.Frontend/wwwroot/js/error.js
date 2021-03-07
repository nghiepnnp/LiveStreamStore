var error = {
    success: { Code: 0, Message: "thành công." },
    failed: { Code: 1, Message: "Thất bại." },
    system: { Code: 2, Message: "Lỗi hệ thống." },
    invalid_data: { Code: 5, Message: "Dữ liệu vào không chính xác." },
    check_product: { Code: 9, Message: "Số lượng còn lại không đủ hoặc đã bán hết." },
    customer_invalid: { Code: 120, Message: "Tài khoản hoặc mật khẩu không chính xác." },
    customer_email_existed: { Code: 121, Message: "Email đã tồn tại." },
    phone_existed: { Code: 122, Message: "Số điện thoại đã tồn tại." },
    limited: { Code: 124, Message: "Số lượng bạn thêm vượt quá giới hạn được mua." }
}