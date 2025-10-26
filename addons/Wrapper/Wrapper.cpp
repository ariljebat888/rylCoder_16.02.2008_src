// This is the main DLL file.

#include <windows.h>
#include "Wrapper.h"
#include "IL/il.h"
#include "minilzo.h"
#pragma warning( disable : 4091 )
#include <msclr\lock.h>
#pragma warning( default : 4091 )

static FischR::Wrapper::Wrapper() {
	ilInit();
	_mutexLock = gcnew Object();
}
//--  --//
System::Drawing::Bitmap^ FischR::Wrapper::LoadDDS(System::String^ file) {
	return LoadDDS(file, System::Drawing::Rectangle(0, 0, 0, 0));
}
System::Drawing::Bitmap^ FischR::Wrapper::LoadDDS(System::String^ file, System::Drawing::Rectangle area) {
	if (!System::IO::File::Exists(file)) return nullptr;
	try {
		return LoadDDS(System::IO::File::ReadAllBytes(file), area);
	} catch (System::Exception^) {
		return nullptr;
	}
}
System::Drawing::Bitmap^ FischR::Wrapper::LoadDDS(cli::array<System::Byte>^ data, System::Drawing::Rectangle area) {
	msclr::lock l(_mutexLock);
	//data[0] = 0x44;
	//data[1] = 0x44;
	//data[2] = 0x53;
	//data[3] = 0x20;
	DWORD dwImage;
	dwImage = ilGenImage();
	ilBindImage(dwImage);
	cli::pin_ptr<System::Byte> pData = &data[0];
	if (ilLoadL(IL_DDS, pData, data->Length) == FALSE) {
		ilDeleteImage(dwImage);
		return nullptr;
	}
	if (area.Width == 0) area.Width = ilGetInteger(IL_IMAGE_WIDTH);
	if (area.Height == 0) area.Height = ilGetInteger(IL_IMAGE_HEIGHT);
	ilConvertImage(IL_BGRA, IL_UNSIGNED_BYTE);
	System::Drawing::Bitmap^ result = gcnew System::Drawing::Bitmap(area.Width, area.Height, System::Drawing::Imaging::PixelFormat::Format32bppArgb);
	System::Drawing::Imaging::BitmapData^ bmpData = 
		result->LockBits(System::Drawing::Rectangle(0, 0, area.Width, area.Height),
		System::Drawing::Imaging::ImageLockMode::ReadWrite,
		System::Drawing::Imaging::PixelFormat::Format32bppArgb);
	ilCopyPixels(area.X, area.Y, 0, area.Width, area.Height, 1, IL_BGRA, IL_UNSIGNED_BYTE, (void*)bmpData->Scan0);
	result->UnlockBits(bmpData);
	ilDeleteImage(dwImage);
	return result;
}
//--  --//
System::Drawing::Bitmap^ FischR::Wrapper::LoadMapDDS(System::String^ fileMask) {
	System::Drawing::Bitmap^ result;
	System::Drawing::Imaging::BitmapData^ bmpData;
	try {
		result = gcnew System::Drawing::Bitmap(0xAFF, 0xAFF,
			System::Drawing::Imaging::PixelFormat::Format32bppArgb);
		bmpData = result->LockBits(
			System::Drawing::Rectangle(0,0,0xAFF,0xAFF),
			System::Drawing::Imaging::ImageLockMode::WriteOnly, result->PixelFormat);
		int failed = 0;
		for (WORD x=0; x<11; ++x) {
			for (WORD y=0; y<11; ++y) {
				cli::array<unsigned char>^ data = nullptr;
				System::String^ file = System::String::Format(fileMask, x+1, y+1);
				if (System::IO::File::Exists(file))
				{
					try
					{
						data = System::IO::File::ReadAllBytes(file);
					}
					catch (System::Exception^) {}
				}
				if (data != nullptr)
				{
					if (!LoadDDS(data, bmpData, System::Drawing::Rectangle(x*0xFF, y*0xFF, 0xFF, 0xFF)))
						failed++;
				}
				else failed++;
			}
		}
		if (failed == 11*11) goto Failed;

		result->UnlockBits(bmpData);
		result->RotateFlip(System::Drawing::RotateFlipType::RotateNoneFlipY);
		return result;
	} catch (System::Exception^) {}
Failed:
	if (result != nullptr && bmpData != nullptr) result->UnlockBits(bmpData);
	return nullptr;
}
System::Boolean FischR::Wrapper::LoadDDS(cli::array<System::Byte>^ data, System::Drawing::Imaging::BitmapData^ bmpData, System::Drawing::Rectangle area) {
	msclr::lock l(_mutexLock);
	DWORD dwImage;
	dwImage = ilGenImage();
	ilBindImage(dwImage);
	cli::pin_ptr<System::Byte> pData = &data[0];
	if (ilLoadL(IL_DDS, pData, data->Length) == FALSE) {
		ilDeleteImage(dwImage);
		return false;
	}
	INT32 width = ilGetInteger(IL_IMAGE_WIDTH);
	INT32 height = ilGetInteger(IL_IMAGE_HEIGHT);
	ilConvertImage(IL_BGRA, IL_UNSIGNED_BYTE);
	int pixSize = System::Drawing::Image::GetPixelFormatSize(bmpData->PixelFormat) / 8;
	System::Byte* pSrc = ilGetData();
	System::Byte* pDst = (System::Byte*)(void*) bmpData->Scan0;
	pDst += (bmpData->Width*area.Y + area.X)*pixSize;
	INT32 srcWidth = width * pixSize;
	INT32 dstWidth = area.Width*pixSize;
	width = (srcWidth > dstWidth ? dstWidth : srcWidth);
	dstWidth = bmpData->Width*pixSize;
	for (int i=0; i<height && i<area.Height; ++i) {
		memcpy(pDst,pSrc,width);
		pDst += dstWidth;
		pSrc += srcWidth;
	}
	ilDeleteImage(dwImage);
	return true;
}
//-- old --//
System::Boolean FischR::Wrapper::LoadDDS(System::String^ file, System::Drawing::Imaging::BitmapData^ bmpData, System::Drawing::Rectangle area) {
	msclr::lock l(_mutexLock);
	cli::array<System::Byte>^ data;
	DWORD dwImage;

	data = System::IO::File::ReadAllBytes(file);
	data[0] = 0x44;
	data[1] = 0x44;
	data[2] = 0x53;
	data[3] = 0x20;

	dwImage = ilGenImage();
	ilBindImage(dwImage);
	cli::pin_ptr<System::Byte> pData = &data[0];
	if (ilLoadL(IL_DDS, pData, data->Length) == FALSE) {
		ilDeleteImage(dwImage);
		return false;
	}
	INT32 width = ilGetInteger(IL_IMAGE_WIDTH);
	INT32 height = ilGetInteger(IL_IMAGE_HEIGHT);
	ilConvertImage(IL_BGRA, IL_UNSIGNED_BYTE);
	int pixSize = System::Drawing::Image::GetPixelFormatSize(bmpData->PixelFormat) / 8;
	System::Byte* pSrc = ilGetData();
	System::IntPtr x = bmpData->Scan0;
	System::Byte* y = (System::Byte*)x.ToPointer();
	cli::pin_ptr<System::Byte> pDst = y;
	pDst += (bmpData->Width*area.Bottom + area.X)*pixSize;
	INT32 srcWidth = width * pixSize;
	INT32 dstWidth = area.Width*pixSize;
	width = (srcWidth > dstWidth ? dstWidth : srcWidth);
	dstWidth = bmpData->Width*pixSize;
	for (int i=0; i<height && i<area.Height; ++i) {
		memcpy(x.ToPointer(),pSrc,width);
		pDst -= dstWidth;
		pSrc += srcWidth;
	}
	ilDeleteImage(dwImage);
	return true;
}
System::Boolean FischR::Wrapper::LoadDDS(System::String^ file, cli::array<System::Byte>^ addTo, System::Drawing::Point place, int imageWidth) {
	msclr::lock l(_mutexLock);

	cli::array<System::Byte>^ data;
	try{
		data = System::IO::File::ReadAllBytes(file);
		data[0] = 0x44;
		data[1] = 0x44;
		data[2] = 0x53;
		data[3] = 0x20;
	}catch(System::Exception^){
		throw;
	}

	DWORD dwImage;

	dwImage = ilGenImage();
	ilBindImage(dwImage);
	cli::pin_ptr<System::Byte> pData = &data[0];
	if (ilLoadL(IL_DDS, pData, data->Length) == FALSE) throw gcnew System::SystemException(gcnew System::String("DDS file load failed"));
	ilConvertImage(0x80e1, 0x1401);
	DWORD width = ilGetInteger(IL_IMAGE_WIDTH);
	DWORD height = ilGetInteger(IL_IMAGE_HEIGHT);

	cli::array<System::Byte>^ bData = gcnew cli::array<System::Byte>(4*width*height);
	cli::pin_ptr<System::Byte> pbData = &bData[0];

	ilCopyPixels(0, 0, 0, width, height, 1, 0x80e1, 0x1401, pbData);

	int num = 0;
    for (int i = height - 1; i >= 0; i--)
    {
        for (int j = 0; j < width; j++)
        {
            int index = (((i + place.Y) * imageWidth) * 4) + ((j + place.X) * 4);

            addTo[index] = bData[num++];
            addTo[index+1] = bData[num++];
            addTo[index+2] = bData[num++];
			addTo[index+3] = bData[num++];
        }
    }
	ilDeleteImage(dwImage);
	delete bData;
	return true;
}
//-- minilzo --//
cli::array<System::Byte>^ FischR::Wrapper::Decompress(cli::array<System::Byte>^ data, System::UInt32 size) {
	cli::pin_ptr<System::Byte> pData = &data[0];
	cli::array<System::Byte>^ result = gcnew cli::array<System::Byte>(size);
	cli::pin_ptr<System::Byte> pResult = &result[0];
	System::UInt32 newSize = size;
	cli::pin_ptr<System::UInt32> pSize = &newSize;
	if (lzo1x_decompress((const unsigned char*)pData,data->Length,
		(unsigned char*)pResult, (lzo_uint*)pSize, 0) != LZO_E_OK ||
		newSize != size) return nullptr;
	return result;
}

cli::array<System::Byte>^ FischR::Wrapper::Compress(cli::array<System::Byte>^ data) {
	cli::pin_ptr<System::Byte> pData = &data[0];
	cli::array<System::Byte>^ result = gcnew cli::array<System::Byte>(data->Length + data->Length / 64 + 16 + 3 + 4);
	cli::pin_ptr<System::Byte> pResult = &result[0];
	System::UInt32 newSize = 0;
	cli::pin_ptr<System::UInt32> pSize = &newSize;
	if (lzo1x_1_compress((const unsigned char*)pData,data->Length,
		(unsigned char*)pResult, (lzo_uint*)pSize, 0) != LZO_E_OK) return nullptr;
	System::Array::Resize<System::Byte>(result, newSize);
	return result;
}
