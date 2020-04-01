using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tao.FFmpeg;
using System.Reflection;
using System.Runtime.InteropServices;
using System.IO;

namespace Golem2.Manager.DataHandling
{
    class oldVideoProcesor
    {
        public oldVideoProcesor(String pathToVideo)
        {
            //FFmpeg.avcodec_init();
            //FFmpeg.av_register_all();

            //IntPtr pFormatContext;

            //int ret = FFmpeg.av_open_input_file(out pFormatContext, pathToVideo, IntPtr.Zero, 0, IntPtr.Zero);

            //if (ret < 0)
            //    throw new Exception("Could not open input file");

            //ret = FFmpeg.av_find_stream_info(pFormatContext);

            //if (ret < 0)
            //    throw new Exception("Could not find stream information");

            //FFmpeg.AVFormatContext formatContext = (FFmpeg.AVFormatContext)Marshal.PtrToStructure(pFormatContext, typeof(FFmpeg.AVFormatContext));

            //for (int i = 0; i < formatContext.nb_streams; i++)
            //{
            //    FFmpeg.AVStream stream = (FFmpeg.AVStream)Marshal.PtrToStructure(formatContext.streams[i], typeof(FFmpeg.AVStream));

            //    FFmpeg.AVCodecContext codec = (FFmpeg.AVCodecContext)Marshal.PtrToStructure(stream.codec, typeof(FFmpeg.AVCodecContext));

            //    if (codec.codec_type == FFmpeg.CodecType.CODEC_TYPE_VIDEO)
            //    {
            //        IntPtr pVideoCodec = FFmpeg.avcodec_find_decoder(codec.codec_id);

            //        if (pVideoCodec == IntPtr.Zero)
            //            throw new Exception("Could not find video codec");

            //        FFmpeg.avcodec_open(stream.codec, pVideoCodec);

            //        IntPtr pFrame = FFmpeg.avcodec_alloc_frame();
            //        IntPtr newFrame = FFmpeg.avcodec_alloc_frame();

            //        int size = codec.width * codec.height;

                    //FFmpeg.avcodec_encode_video(pVideoCodec, 

                    //int numBytes = FFmpeg.avpicture_get_size((int)FFmpeg.PixelFormat.PIX_FMT_RGB24, codec.width, codec.height);

                    /*
                     * uint8_t *buffer;
int numBytes;
// Determine required buffer size and allocate buffer
numBytes=avpicture_get_size(PIX_FMT_RGB24, pCodecCtx->width,
                            pCodecCtx->height);
buffer=(uint8_t *)av_malloc(numBytes*sizeof(uint8_t));
                     * */

            //        IntPtr pPacket = Marshal.AllocHGlobal(56);

            //        int k = 0;
            //        int frameFinished = 0;
            //        while (FFmpeg.av_read_frame(pFormatContext, pPacket) >= 0)
            //        {

            //            FFmpeg.AVPacket packet = (FFmpeg.AVPacket)Marshal.PtrToStructure(pPacket, typeof(FFmpeg.AVPacket));
                        
            //            // Is this a packet from the video stream?
            //            if (packet.stream_index == i)
            //            {
            //                // Decode video frame
            //                FFmpeg.avcodec_decode_video(pVideoCodec, pFrame, ref frameFinished, packet.data, packet.size);

            //                // Did we get a video frame?
            //                if (frameFinished != 0)
            //                {
            //                    // Convert the image from its native format to RGB
            //                    FFmpeg.img_convert(newFrame, (int)FFmpeg.PixelFormat.PIX_FMT_RGB24, pFrame, (int)codec.pix_fmt, codec.width, codec.height);

            //                    // Save the frame to disk
            //                    if (++k <= 5)
            //                        SaveFrame(newFrame, codec.width, codec.height, k);
            //                }
            //            }

            //            // Free the packet that was allocated by av_read_frame
            //            FFmpeg.av_free_packet(pPacket);
            //        }

            //        FFmpeg.av_free(newFrame);
            //        FFmpeg.av_free(pFrame);
            //        FFmpeg.avcodec_close(pVideoCodec);
            //        FFmpeg.av_close_input_file(pFormatContext);
            //    }
            //}

            Init(pathToVideo);
        }

        private void Init(String pathToVideo)
        {

            FFmpegSharp.Video.VideoDecoderStream stream = new FFmpegSharp.Video.VideoDecoderStream(pathToVideo);

            byte[] frame;
            stream.ReadFrame(out frame);
            SaveFrame(frame, stream.Width, stream.Height);
        }

        public Stream GetStream(String pathToVideo)
        {
            return new FFmpegSharp.Video.VideoDecoderStream(pathToVideo);
        }

        public void SaveFrame(byte[] pFrame, int width, int height)
        {
            StreamWriter stream = File.CreateText(@"C:\inetpub\wwwroot\Cache\000001.ppm");

            stream.WriteLine("P3");
            stream.WriteLine(width + " " + height);
            stream.WriteLine("255");
            stream.WriteLine();

            


            //for (int y = 0; y < height; y++)
            //{
            //    stream.Write(frameData + y * frame.linesize[0]);
            //    stream.Write(frameData + y * frame.linesize[0]);
            //    stream.Write(frameData + y * frame.linesize[0]);
            //}
            

            stream.Close();
        }

        void Main(String pathToVideo)
        {
            FFmpeg.avcodec_init();
            FFmpeg.av_register_all();

            // find the h264 video encoder
            IntPtr pcodec = FFmpeg.avcodec_find_encoder(FFmpeg.CodecID.CODEC_ID_MPEG1VIDEO);

            if (pcodec == IntPtr.Zero)
            {
                Console.WriteLine("codec not found");
                return;
            }

            IntPtr pcontext = FFmpeg.avcodec_alloc_context();
            IntPtr ppicture = FFmpeg.avcodec_alloc_frame();

            FFmpeg.AVCodecContext context = (FFmpeg.AVCodecContext)Marshal.PtrToStructure(pcontext, typeof(FFmpeg.AVCodecContext));
            // put sample parameters
            context.bit_rate = 400000;
            // resolution must be a multiple of two
            context.width = 352;
            context.height = 288;
            // frames per second
            context.time_base.den = 25;//(AVRational){1,25};
            context.time_base.num = 1;
            context.gop_size = 10; // emit one intra frame every ten frames
            context.max_b_frames = 1;
            context.pix_fmt = FFmpeg.PixelFormat.PIX_FMT_YUV420P;
            Marshal.StructureToPtr(context, pcontext, false);

            // open it
            if (FFmpeg.avcodec_open(pcontext, pcodec) < 0)
            {
                Console.WriteLine("could not open codec");
                return;
            }

            // alloc image and output buffer
            int outbuf_size = 100000;
            byte[] outbuf = new byte[outbuf_size];
            int size = context.width * context.height;
            byte[] picture_buf = new byte[(size * 3) / 2]; // size for YUV 420

            FFmpeg.AVFrame picture = (FFmpeg.AVFrame)Marshal.PtrToStructure(ppicture, typeof(FFmpeg.AVFrame));

            picture.data[0] = Marshal.UnsafeAddrOfPinnedArrayElement(picture_buf, 0);
            picture.data[1] = Marshal.UnsafeAddrOfPinnedArrayElement(picture_buf, size); // picture.data[0] + size;
            picture.data[2] = Marshal.UnsafeAddrOfPinnedArrayElement(picture_buf, size + size / 4); // picture.data[1] + size / 4;
            picture.linesize[0] = context.width;
            picture.linesize[1] = context.width / 2;
            picture.linesize[2] = context.width / 2;

            int data1offset = size;
            int data2offset = size + size / 4;

            Marshal.StructureToPtr(picture, ppicture, false);

            System.IO.FileStream fstream = new System.IO.FileStream(@"C:\inetpub\wwwroot\Cache\000001.mp4", System.IO.FileMode.Create);

            int i, out_size = 0, x, y;

            // encode 1 second of video
            for (i = 0; i < 250; i++)
            {
                // prepare a dummy image
                // Y
                for (y = 0; y < context.height; y++)
                {
                    for (x = 0; x < context.width; x++)
                    {
                        picture_buf[y * picture.linesize[0] + x] = (byte)(x + y + i * 3); // picture.data[0][y * picture.linesize[0] + x] = x + y + i * 3;
                    }
                }

                 // Cb and Cr
                for (y = 0; y < context.height / 2; y++)
                {
                    for (x = 0; x < context.width / 2; x++)
                    {
                        picture_buf[data1offset + y * picture.linesize[1] + x] = (byte)(128 + y + i * 2); // picture.data[1][y * picture.linesize[1] + x] = 128 + y + i * 2;
                        picture_buf[data2offset + y * picture.linesize[2] + x] = (byte)(64 + x + i * 5); // picture.data[2][y * picture.linesize[2] + x] = 64 + x + i * 5;
                    }
                }

                // encode the image
                out_size = FFmpeg.avcodec_encode_video(pcontext, Marshal.UnsafeAddrOfPinnedArrayElement(outbuf, 0), outbuf_size, ppicture);
                Console.WriteLine("encoding frame {0:D3} (size={1:D5})", i, out_size);
                fstream.Write(outbuf, 0, out_size); // fwrite(outbuf, 1, out_size, f);
            }

            // get the delayed frames
            for (; out_size > 0; i++)
            {
                out_size = FFmpeg.avcodec_encode_video(pcontext, Marshal.UnsafeAddrOfPinnedArrayElement(outbuf, 0), outbuf_size, IntPtr.Zero);
                Console.WriteLine("encoding frame {0:D3} (size={1:D5})", i, out_size);
                fstream.Write(outbuf, 0, out_size); // fwrite(outbuf, 1, out_size, f);
            }

            // add sequence end code to have a real mpeg file
            outbuf[0] = 0x00;
            outbuf[1] = 0x00;
            outbuf[2] = 0x01;
            outbuf[3] = 0xb7;
            fstream.Write(outbuf, 0, 4); // fwrite(outbuf, 1, 4, f);
            fstream.Close();

            FFmpeg.avcodec_close(pcontext);
            FFmpeg.av_free(pcontext);
            FFmpeg.av_free(ppicture);
        }



    }

}
