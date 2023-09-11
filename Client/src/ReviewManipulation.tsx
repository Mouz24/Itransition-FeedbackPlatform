import axios, { isAxiosError } from 'axios';
import React, { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import axiosInstance from './AxiosInstance';
import Box from '@mui/material/Box';
import TextField from '@mui/material/TextField';
import Button from '@mui/material/Button';
import { useUserContext } from './UserContext';
import { ReviewDTO } from './EntitiesDTO';
import ReactQuill from 'react-quill';
import FormControl from '@mui/material/FormControl';
import InputLabel from '@mui/material/InputLabel';
import Select, { SelectChangeEvent } from '@mui/material/Select';
import MenuItem from '@mui/material/MenuItem';
import { Group } from './Entities';
import { useDropzone } from 'react-dropzone';

const ReviewManipulation: React.FC = () => {
  const navigate = useNavigate();
  const { setLoggedInUser } = useUserContext();
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const { userId } = useParams<{ userId: string }>();
  const [groupOptions, setGroupOptions] = useState<Group[]>([]);
  const [mark, setMark] = useState('');
  const [group, setGroup] = useState('');
  const [imageFiles, setImageFiles] = useState<File[]>([]);
  const { getRootProps, getInputProps, isDragActive } = useDropzone({
    accept: {
      'image/*': []
    },
    onDrop: (acceptedFiles) => {
      const newImageFiles = [...imageFiles, ...acceptedFiles];
      setImageFiles(newImageFiles);
    }
  });

  const FilePreview = ({ files }: { files: File[] }) => (
    <div>
      {files.map((file, index) => (
        <div key={index}>
          <img
            src={URL.createObjectURL(file)}
            alt={`File ${index}`}
            width="100"
            height="100"
          />
        </div>
      ))}
    </div>
  );

  const [formData, setFormData] = useState<ReviewDTO>({
    title: '',
    text: '',
    mark: undefined,
    artworkName: '',
    groupId: undefined,
    userId: userId,
    error: ''
  });

  const [fieldErrors, setFieldErrors] = useState({
    title: '',
    text: '',
    mark: '',
    artworkName: '',
    group: '',
  });

  useEffect(() => {
    fetchGroupOptions();
  }, []);

  const fetchGroupOptions = async () => {
    try {
      const response = await axiosInstance.get('group');
      setGroupOptions(response.data);
    } catch (error) {
      console.error('Error fetching group options:', error);
    }
  };

  const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = event.target;
    setFormData((prevFormData) => ({
      ...prevFormData,
      [name]: value,
      error: '',
    }));

    setFieldErrors((prevFieldErrors) => ({
      ...prevFieldErrors,
      [name]: '',
    }));
  };

  const handleQuillChange = (value: string) => {
    setFormData((prevFormData) => ({
      ...prevFormData,
      text: value,
      error: '',
    }));
  };

  const handleMarkChange = (event: SelectChangeEvent) => {
    const value = Number(event.target.value);
    if (!isNaN(value) && value >= 1 && value <= 10) {
      setMark(event.target.value);
      setFormData((prevFormData) => ({
        ...prevFormData,
        mark: value,
        error: '',
      }));
    }
  };

  const handleGroupChange = (event: SelectChangeEvent) => {
    const groupName = event.target.value;
    const selectedGroup = groupOptions.find((group) => group.name === groupName);

    setGroup(groupName);

    if (selectedGroup) {
      setFormData((prevFormData) => ({
        ...prevFormData,
        groupId: selectedGroup.id,
        error: '',
      }));
    }
  };

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();

    if (isLoading) return;

    setIsLoading(true);

    try {
      const response = await axios.post(`http://localhost:5164/api/review/${userId}`, {formData, imageFiles: imageFiles});
      setIsLoading(false);
      navigate(`/${userId}/reviews/${response.data.Id}`);
    } catch (error: any) {
      if (isAxiosError(error)) {
        if (error.response) {
          const responseData = error.response.data;
          if (responseData) {
            setFieldErrors({
              title: responseData.Title?.[0] || '',
              text: responseData.Text?.[0] || '',
              artworkName: responseData.ArtworkName[0] || '',
              mark: responseData.Mark?.[0] || '',
              group: responseData.GroupId?.[0] || '',
            });
          }
        } else {
          console.error(
            'Unknown Error:',
            'An unknown error occurred during registration.'
          );
          setFormData((prevFormData) => ({
            ...prevFormData,
            error: 'An unknown error occurred during registration.',
          }));
        }
      }
    }
  };

  return (
    <Box
      component="form"
      sx={{
        '& .MuiTextField-root': { m: 1, width: '25ch' },
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
      }}
      noValidate
      autoComplete="on"
      onSubmit={handleSubmit}
    >
    <div>
      <TextField
        label={formData.error ? 'Error' : 'Title'}
        variant="outlined"
        type="text"
        name="title"
        value={formData.title}
        onChange={handleChange}
        error={Boolean(formData.error)}
        helperText={formData.error || ''}
      />
    </div>
    <div>
      <label htmlFor="text">Review:</label>
      <ReactQuill id="text" value={formData.text} onChange={handleQuillChange} />
      <div>
        <strong>Preview:</strong>
        <div dangerouslySetInnerHTML={{ __html: formData.text }} />
      </div>
    </div>
    <div>
      <FormControl>
        <InputLabel htmlFor="mark">Mark</InputLabel>
        <Select
          id="mark"
          name="mark"
          value={mark}
          onChange={handleMarkChange}
          variant="outlined"
        >
          {Array.from({ length: 10 }, (_, i) => i + 1).map((value) => (
            <MenuItem key={value} value={String(value)}>
              {value}
            </MenuItem>
          ))}
        </Select>
      </FormControl>
      <div style={{ color: 'red', fontSize: '0.8rem', marginTop: '0.5rem' }}>
        {fieldErrors.mark}
      </div>
    </div>
    <div>
      <FormControl>
        <InputLabel htmlFor="group">Group</InputLabel>
        <Select
          id="group"
          name="group"
          value={group}
          onChange={handleGroupChange}
          variant="outlined"
        >
          <MenuItem value="0">Select a Group</MenuItem>
          {groupOptions.map((groupOption, index) => (
            <MenuItem key={index} value={groupOption.name}>
              {groupOption.name}
            </MenuItem>
          ))}
        </Select>
      </FormControl>
      <div style={{ color: 'red', fontSize: '0.8rem', marginTop: '0.5rem' }}>
        {fieldErrors.group}
      </div>
    </div>
    <div {...getRootProps()} className="dropzone">
      <input {...getInputProps()} />
      {isDragActive ? (
        <p>Drop the image here...</p>
      ) : (
        <p>Drag &amp; drop an image here, or click to select one</p>
      )}
    </div>
    <div>
        <FilePreview files={imageFiles} />
    </div>
    <div>
      <Button variant="contained" color="success" type="submit">
        Create
      </Button>
    </div>
  </Box>
  );
};

export default ReviewManipulation;
