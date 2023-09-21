import React, { useState } from 'react';
import Dropzone from 'react-dropzone';
import { getAvatarContent, useUserContext } from '../components/UserContext';
import { Avatar, Box, Button, Typography } from '@mui/material';
import axiosInstance from '../components/AxiosInstance';
import { useNavigate } from 'react-router-dom';

const AvatarUpload: React.FC = () => {
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [previewUrl, setPreviewUrl] = useState<string | null>(null);
  const { loggedInUser, setLoggedInUser } = useUserContext();
  const navigate = useNavigate();

  const onDrop = (acceptedFiles: File[]) => {
    const file = acceptedFiles[0];
    setSelectedFile(file);

    const reader = new FileReader();
    reader.onload = () => {
      setPreviewUrl(reader.result as string);
    };
    reader.readAsDataURL(file);
  };

  const clearSelection = () => {
    setSelectedFile(null);
    setPreviewUrl(null);
  };

  const uploadAvatar = async () => {
    if (!selectedFile) return;

    const formData = new FormData();
    formData.append('image', selectedFile);

    try {
      const response = await axiosInstance.post(`users/${loggedInUser?.id}/avatar`, formData, {
        headers: {
          'Content-Type': 'multipart/form-data',
        },
      });

      if (loggedInUser) {
        const updatedUser = { ...loggedInUser, avatar: response.data };
        setLoggedInUser(updatedUser);
  
        localStorage.setItem('user', JSON.stringify(updatedUser));
      }

      setSelectedFile(null);
      navigate('/');
    } catch (error) {
      console.error('Error uploading avatar:', error);
    }
  };

  return (
    <Box sx={{display: 'flex', justifyContent: 'center', alignItems: 'center', flexDirection: 'column'}}>
      <h1>Avatar Upload</h1>
      <Box sx={{display: 'flex', flexDirection: 'column', alignItems: 'center'}}>
        <Box sx={{display: 'flex', gap: '10px', alignItems: 'center'}}>
            <Typography variant='h6'>Current Avatar:</Typography>
            {getAvatarContent(loggedInUser)}
        </Box>
        {selectedFile ? (
            <Box>
                <Box sx={{display: 'flex', gap: '10px', alignItems: 'center'}}>
                    <Typography variant='h4'>New Avatar:</Typography>
                    {previewUrl && (
                        <Avatar src={previewUrl} alt="Selected File Preview" style={{ width: '150px', height: '150px' }} />
                    )}
                </Box>
                <Box sx={{display: 'flex', maxWidth: '200px', flexDirection: 'column', gap: '10px'}}>
                    <Button variant='outlined' onClick={clearSelection}>Clear Selection</Button>
                    <Button variant='outlined' onClick={uploadAvatar}>Upload</Button>
                </Box>
            </Box>
      ) : (
        <Dropzone onDrop={onDrop}>
          {({ getRootProps, getInputProps }) => (
            <div {...getRootProps()} style={dropzoneStyle}>
              <input {...getInputProps()} />
              <p>Drag &amp; drop your new avatar here, or click to select one</p>
            </div>
          )}
        </Dropzone>
      )}
      </Box>
    </Box>
  );
};

const dropzoneStyle: React.CSSProperties = {
  border: '2px dashed #cccccc',
  borderRadius: '4px',
  padding: '20px',
  textAlign: 'center',
  cursor: 'pointer',
  marginTop: '10px',
};

export default AvatarUpload;
