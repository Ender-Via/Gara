# Gara - README (MVVM)

Tài liệu ngắn cho thành viên mới nắm nhanh cấu trúc và cách mở rộng dự án theo hướng MVVM.

## 1) Cây thư mục (hiện trạng)
```text
WpfApp1/
├─ Views/
│  ├─ MainWindow.xaml (+ MainWindow.xaml.cs)
│  ├─ TiepNhanWindow.xaml (+ TiepNhanWindow.xaml.cs)
│  ├─ PhieuSuaChuaWindow.xaml (+ PhieuSuaChuaWindow.xaml.cs)
│  ├─ TraCuuWindow.xaml (+ TraCuuWindow.xaml.cs)
│  ├─ PhieuThuWindow.xaml (+ PhieuThuWindow.xaml.cs)
│  ├─ BaoCaoWindow.xaml (+ BaoCaoWindow.xaml.cs)
│  └─ QuyDinhWindow.xaml (+ QuyDinhWindow.xaml.cs)
├─ ViewModels/
│  ├─ MainViewModel.cs
│  ├─ TiepNhanViewModel.cs
│  ├─ PhieuSuaChuaViewModel.cs
│  ├─ TraCuuViewModel.cs
│  ├─ PhieuThuViewModel.cs
│  ├─ BaoCaoViewModel.cs
│  └─ QuyDinhViewModel.cs
├─ Models/                  (placeholder)
├─ Services/                (placeholder)
├─ Resources/
│  ├─ Styles/               (ResourceDictionary, style/theme)
│  └─ Converters/           (IValueConverter)
├─ Helpers/                 (utilities, validators, dialog helpers)
└─ .gitkeep                 (giữ folder rỗng trong Git)
```

## 2) Ý nghĩa từng phần
- **Views**: chứa giao diện XAML và code-behind tối thiểu (khởi tạo + gán `DataContext`).
- **ViewModels**: chứa trạng thái UI (properties) và command (`ICommand`).
- **Models**: chứa lớp dữ liệu/domain.
- **Services**: chứa logic dùng chung (đọc/ghi dữ liệu, điều hướng, dialog...).
- **Resources/Styles**: style, theme và resource dùng lại.
- **Resources/Converters**: converter cho binding.
- **Helpers**: hàm tiện ích và helper dùng chung.

## 3) Quy ước MVVM đang dùng
- Mỗi màn hình trong `Views` có một ViewModel tương ứng trong `ViewModels`.
- Dùng `CommunityToolkit.Mvvm`:
  - `[ObservableProperty]` cho trạng thái bindable.
  - `[RelayCommand]` cho thao tác nút/lệnh.
- Ưu tiên binding (`Command`, `Text`, `SelectedDate`, `ItemsSource`) thay cho xử lý trực tiếp trong View.

## 4) Cách thêm màn hình mới
1. Tạo `XxxWindow.xaml` trong `Views/`.
2. Tạo `XxxViewModel.cs` trong `ViewModels/`.
3. Gán `DataContext = new XxxViewModel()` trong code-behind của `XxxWindow`.
4. Bind UI vào property/command trong ViewModel.

## 5) Lưu ý hiển thị tiếng Việt
- Lưu file `.cs/.xaml/.resx` bằng UTF-8 (khuyến nghị UTF-8 with BOM nếu gặp lỗi hiển thị).
- Tránh font không hỗ trợ đầy đủ tiếng Việt.

## 6) Trạng thái hiện tại
- Đã tách theo cấu trúc `Views` + `ViewModels`.
- Đã có thư mục nền cho `Models`, `Services`, `Resources`, `Helpers`.
- Có thể tiếp tục chuẩn hóa dần code-behind theo từng màn hình.