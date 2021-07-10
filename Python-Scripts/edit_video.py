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
image = sys.argv[4] 
position = sys.argv[5]
start_seconds = sys.argv[6]
end_seconds = sys.argv[7]
is_percentage = sys.argv[8]

# GET IMAGE DIMENSIONS
imageOpen = PIL.Image.open(image)
img_width, img_height = imageOpen.size

# PX TO PUSH THE IMAGE AWAY FROM THE CORNERS
static_px_push = '20'

# DEFINE PICTURE POSITION FORMULAS
width_position_left = "W-W+"+static_px_push
width_position_middle = "W/2-"+str(img_width)+"/2"
width_position_right = "W-"+str(img_width)+"-"+static_px_push
height_position_top = "H-H+"+static_px_push
height_position_middle = "H/2-"+str(img_height)+"/2"
height_position_bottom = "H-"+str(img_height)+"-"+static_px_push

# ASSING POSITION FORMULAS IN DICTIONARY, (KEY = POSITION) (VALUE = FORMULA)
dict_position = { "Top Left":width_position_left+':'+height_position_top,"Middle Left":width_position_left+':'+height_position_middle,"Bottom Left":width_position_left+':'+height_position_bottom,"Top":width_position_middle+':'+height_position_top,"Middle":width_position_middle+':'+height_position_middle,"Bottom":width_position_middle+':'+height_position_bottom,"Top Right":width_position_right+':'+height_position_top,"Middle Right":width_position_right+':'+height_position_middle,"Bottom Right":width_position_right+':'+height_position_bottom}
video_count = 0

# EDIT VIDEOS FROM FILES
if files_path != "":
        for file_path in files_path.split(",seperated,"):
            filename = file_path.split('\\')[-1]
            print('current-video:',filename)
            print("videos-completed:",video_count)
            video_duration = mp.VideoFileClip(file_path).duration

            if is_percentage == 'True':
                start_seconds_final = (int(start_seconds) / 100) * video_duration
                end_seconds_final = (int(end_seconds) / 100) * video_duration
            else:
                start_seconds_final = start_seconds
                end_seconds_final = end_seconds
            if int(end_seconds) == 0 or int(end_seconds) < int(start_seconds): # IF END SECONDS ARE NOT SPECIFIED OR BIGGER THAN START, GO UNTIL THE END
                    end_seconds_final = video_duration

            subprocess.run('ffmpeg -y -i "{file_path}" -i "{img}" -filter_complex "[0:v][1:v] overlay={position}:enable=\'between(t,{start_seconds},{end_seconds})\'" -vcodec libx264 -profile:v high444 -refs 14 -preset ultrafast -crf 20 -tune fastdecode {copyto}\EDITED_{count}_"{filename}"'.format(
                file_path=file_path, img=image, copyto=copy_to, count=count,filename = filename,position = dict_position[position],
                start_seconds = start_seconds_final, end_seconds = end_seconds_final))
            p = subprocess.Popen(
                "echo 'foo' && sleep 5 && echo 'bar'", shell=True)

            video_count = video_count + 1
            print("videos-completed:",video_count)
            count = count + 1
        p.terminate()  

# EDIT VIDEOS FROM FOLDER
if folder_path != "":
    for subdir, dirs, files in os.walk(folder_path):
        for file in files:
            file_path = os.path.join(subdir, file)
            print('current-video:',file)
            print("videos-completed:",video_count)
            video_duration = mp.VideoFileClip(file_path).duration
            
            if is_percentage == 'True':
                start_seconds_divided = (int(start_seconds) / 100) * video_duration
                end_seconds_divided = (int(end_seconds) / 100) * video_duration
            else:
                start_seconds_divided = start_seconds
                end_seconds_divided = end_seconds
            if int(end_seconds) == 0 or int(end_seconds) < int(start_seconds):
                    end_seconds_divided = video_duration

            subprocess.run('ffmpeg -y -i "{file_path}" -i "{img}" -filter_complex "[0:v][1:v] overlay={position}:enable=\'between(t,{start_seconds},{end_seconds})\'" -vcodec libx264 -profile:v high444 -refs 14 -preset ultrafast -crf 20 -tune fastdecode {copyto}\EDITED_{count}_"{filename}"'.format(
                file_path=file_path, img=image, copyto=copy_to, count=count,filename = file,img_width = img_width,img_height = img_height,position = dict_position[position],
                start_seconds = start_seconds_divided, end_seconds = end_seconds_divided))

            p = subprocess.Popen(
                "echo 'foo' && sleep 60 && echo 'bar'", shell=True)
            
            video_count = video_count + 1
            print("videos-completed:",video_count)
            count = count + 1
        p.terminate()