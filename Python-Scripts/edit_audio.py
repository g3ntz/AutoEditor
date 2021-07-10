import os
import subprocess
import sys
import moviepy.editor as mp
import PIL

count = 0 # INCREMENT EACH EDITED VIDEO NAME 

# SCRIPT PARAMETERS
folder_path = sys.argv[1]
files_path = sys.argv[2]
copy_to = sys.argv[3]
bitrate = sys.argv[4] 
trim_start = sys.argv[5]
trim_end = sys.argv[6]
speed = sys.argv[7]

trim_start_sum = sum([int(x) for x in trim_start.split(':')])
trim_end_sum = sum([int(x) for x in trim_end.split(':')])

trim = '-ss ' + trim_start + ' -to ' + trim_end
if trim_start_sum == 0 and trim_end_sum == 0:
    trim = ''
if trim_end_sum < trim_start_sum:
    trim = '-ss '+trim_start

audio_count = 0

# EDIT AUDIOS FROM FILES
if files_path != "":
    files_path_list = files_path.split(",seperated,")
    for file_path in files_path_list:
        filename = file_path.split('\\')[-1]
        print('current-audio:',filename)
        print("audios-completed:",audio_count)

        subprocess.run('ffmpeg -y -i "{file_path}" -b:a {bitrate}k -filter_complex "atempo={speed}" {trim} {copyto}\EDITED_{count}_"{filename}"'.format(
            file_path=file_path, copyto=copy_to, count=count,filename = filename,bitrate = bitrate, speed = speed,trim = trim))
        p = subprocess.Popen(
            "echo 'foo' && sleep 60 && echo 'bar'", shell=True)
        
        audio_count = audio_count + 1
        print("audios-completed:",audio_count)
        count = count + 1
    p.terminate()
        

# EDIT AUDIOS FROM FOLDER
if folder_path != "":
    for subdir, dirs, files in os.walk(folder_path):
        for file in files:
            file_path = os.path.join(subdir, file)
            print('current-audio:',file)
            print("audios-completed:",audio_count)
            
            subprocess.run('ffmpeg -y -i "{file_path}" -b:a {bitrate}k -filter_complex "atempo={speed}" {trim} {copyto}\EDITED_{count}_"{filename}"'.format(
            file_path=file_path, copyto=copy_to, count=count,filename = file,bitrate = bitrate, speed = speed,trim = trim))
            p = subprocess.Popen(
                "echo 'foo' && sleep 60 && echo 'bar'", shell=True)
            
            audio_count = audio_count + 1
            print("audios-completed:",audio_count)
            count = count + 1
        p.terminate()
            