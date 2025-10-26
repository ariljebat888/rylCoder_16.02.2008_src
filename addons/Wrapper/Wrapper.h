// DevilWrapper.h

#pragma once

namespace FischR {

	public ref class Wrapper {
	private:
		static Wrapper();
		static System::Object^ _mutexLock;
		static System::Drawing::Bitmap^ LoadDDS(cli::array<System::Byte>^ data, System::Drawing::Rectangle area);
		static System::Boolean LoadDDS(cli::array<System::Byte>^ data, System::Drawing::Imaging::BitmapData^ bmpData, System::Drawing::Rectangle area);
	public:
		static System::Drawing::Bitmap^ LoadDDS(System::String^ file);
		static System::Drawing::Bitmap^ LoadDDS(System::String^ file, System::Drawing::Rectangle area);
		static System::Drawing::Bitmap^ LoadMapDDS(System::String^ fileMask);
		static cli::array<System::Byte>^ Decompress(cli::array<System::Byte>^ data, System::UInt32 size);
		static cli::array<System::Byte>^ Compress(cli::array<System::Byte>^ data);
	private:
		// old
		static System::Boolean LoadDDS(System::String^ file, cli::array<System::Byte>^ addTo, System::Drawing::Point place, int imageWidth);
		static System::Boolean LoadDDS(System::String^ file, System::Drawing::Imaging::BitmapData^ bmpData, System::Drawing::Rectangle area);
	};
}
