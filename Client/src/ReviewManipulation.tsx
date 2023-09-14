import axios, { isAxiosError } from 'axios';
import React, { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import axiosInstance from './AxiosInstance';
import Box from '@mui/material/Box';
import TextField from '@mui/material/TextField';
import Button from '@mui/material/Button';
import { useUserContext } from './UserContext';
import { ReviewDTO } from './EntitiesDTO';
import FormControl from '@mui/material/FormControl';
import InputLabel from '@mui/material/InputLabel';
import Select, { SelectChangeEvent } from '@mui/material/Select';
import MenuItem from '@mui/material/MenuItem';
import { Group, Tag } from './Entities';
import { useDropzone } from 'react-dropzone';
import ReactQuill from 'react-quill';
import 'react-quill/dist/quill.snow.css';
import 'react-quill/dist/quill.bubble.css';
import { styled } from '@mui/system';
import { error } from 'console';
import { FormHelperText } from '@mui/material';
import TextInput from 'react-autocomplete-input';
import 'react-autocomplete-input/dist/bundle.css';

const DropzoneContainer = styled('div')({
  border: '2px dashed #cccccc',
  borderRadius: '4px',
  padding: '20px',
  textAlign: 'center',
  cursor: 'pointer',
  marginBottom: '10px',
  '& p': {
    margin: '0',
  },
  '&.active': {
    borderColor: '#007bff',
  },
});

const ReviewManipulation: React.FC = () => {
  const navigate = useNavigate();
  const { setLoggedInUser } = useUserContext();
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const { userId } = useParams<{ userId: string }>();
  const [groupOptions, setGroupOptions] = useState<Group[]>([]);
  const [mark, setMark] = useState('');
  const [availableTags, setAvailableTags] = useState<Tag[]>([]);
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
    <Box sx={{ display: 'flex', flexWrap: 'wrap' }}>
      {files.map((file, index) => (
        <div key={index} style={{ position: 'relative' }}>
          <img
            src={URL.createObjectURL(file)}
            alt={`File ${index}`}
            width="100"
            height="100"
          />
          <button
            onClick={() => handleRemoveImage(index)}
            style={{
              position: 'absolute',
              top: '5px',
              right: '5px',
              background: 'red',
              color: 'white',
              border: 'none',
              borderRadius: '50%',
              padding: '5px',
              cursor: 'pointer',
            }}
          >
            X
          </button>
        </div>
      ))}
    </Box>
  );
  
  const handleRemoveImage = (indexToRemove: number) => {
    const newImageFiles = imageFiles.filter((_, index) => index !== indexToRemove);
    setImageFiles(newImageFiles);
  };

  const [formData, setFormData] = useState<ReviewDTO>({
    title: '',
    text: '',
    mark: null,
    artworkName: '',
    groupId: null,
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
    fetchTags();
  }, []);

  const fetchGroupOptions = async () => {
    try {
      const response = await axiosInstance.get('group');
      setGroupOptions(response.data);
    } catch (error) {
      console.error('Error fetching group options:', error);
    }
  };

  const fetchTags = async () => {
    try {
      const response = await axiosInstance.get('tag');
      setAvailableTags(response.data);
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
      const formDataToSend = new FormData();

      formDataToSend.append('title', formData.title);
      formDataToSend.append('text', formData.text);
      formDataToSend.append('artworkName', formData.artworkName);
      if (formData.userId) {
        formDataToSend.append('userId', formData.userId);
      }
      if (formData.mark) {
        formDataToSend.append('mark', String(formData.mark));
      }
      if (formData.groupId) {
        formDataToSend.append('groupId', String(formData.groupId));
      }

      imageFiles.forEach((file, index) => {
        formDataToSend.append(`imageFiles`, file);
      });

      const tagsInText = formData.text.match(/#(\w+)/g) || [];
      
      const newTags = tagsInText
      .filter(tag => !availableTags.some(existingTag => existingTag.text === tag.substring(1)))
      .map(tag => tag.substring(1));
  
      for (const newTagText of newTags) {
        try {
          const response = await axios.post<Tag>('http://peabody28.com:1030/api/tags', { text: newTagText });
          const newTag = response.data;
  
          setAvailableTags(prevTags => [...prevTags, newTag]);
        } catch (error) {
          console.error(`Error creating new tag "${newTagText}":`, error);
        }
      }

      const response = await axiosInstance.post(`http://localhost:5164/api/review/${userId}`, formDataToSend, {
        headers: {
          'Content-Type': 'multipart/form-data'
        }
      });

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
              artworkName: responseData.ArtworkName?.[0] || '',
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
      <TextField
        label={formData.error ? 'Error' : 'Artwork'}
        variant="outlined"
        type="text"
        name="artworkName"
        value={formData.artworkName}
        onChange={handleChange}
        error={Boolean(formData.error)}
        helperText={formData.error || ''}
      />
    </div>
    <div>
        <ReactQuill
          value={formData.text}
          onChange={handleQuillChange}
          modules={{
            toolbar: [
              [{ header: '1' }, { header: '2' }, { font: [] }],
              ['bold', 'italic', 'underline', 'strike', 'blockquote'],
              [{ color: [] }],
              [{ list: 'ordered' }, { list: 'bullet' }],
              ['clean'],
              ["blockquote", "code-block"],
            ],
          }}
        />
      </div>
    <div>
      <FormControl sx={{width: '100px', marginTop: '10px'}} 
        error={Boolean(fieldErrors.mark)}
      >
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
        <FormHelperText>{fieldErrors.mark}</FormHelperText>
      </FormControl>
    </div>
    <div>
      <FormControl sx={{width: '100px'}}
        error={Boolean(fieldErrors.mark)}
      >
        <InputLabel htmlFor="group">Group</InputLabel>
        <Select
          id="group"
          name="group"
          value={group}
          onChange={handleGroupChange}
          variant="outlined"
        >
          {groupOptions.map((groupOption, index) => (
            <MenuItem key={index} value={groupOption.name}>
              {groupOption.name}
            </MenuItem>
          ))}
        </Select>
        <FormHelperText>{fieldErrors.group}</FormHelperText>
      </FormControl>
    </div>
    <DropzoneContainer
      {...getRootProps()}
      className={isDragActive ? 'active' : ''}
    >
      <input {...getInputProps()} />
      {isDragActive ? (
        <p>Drop the image here...</p>
      ) : (
        <p>Drag &amp; drop an image here, or click to select one</p>
      )}
    </DropzoneContainer>
    <div>
        <FilePreview files={imageFiles}/>
    </div>
    <div>
      <Button variant="contained" color="success" type="submit" sx={{marginTop: '10px'}}>
        Create
      </Button>
    </div>
  </Box>
  );
};

export default ReviewManipulation;
